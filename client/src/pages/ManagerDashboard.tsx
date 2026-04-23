import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { FolderKanban, ListTodo, CheckCircle2, Clock, Users } from 'lucide-react';
import { useAuthStore } from '@/store/useAuthStore';
import { projectsApi, tasksApi, usersApi } from '@/api/services';
import { useLanguage } from '@/hooks/useLanguage';
import StatCard from '@/components/StatCard';
import StatusBadge from '@/components/StatusBadge';

const ManagerDashboard = () => {
  const { t } = useLanguage();
  const { userId } = useAuthStore();

  const { data: projects = [] } = useQuery({
    queryKey: ['manager-projects', userId],
    queryFn: () => projectsApi.getByManager(userId!).then(r => r.data),
    enabled: !!userId,
  });
  const { data: workers = [] } = useQuery({
    queryKey: ['manager-workers', userId],
    queryFn: () => usersApi.getWorkersByManager(userId!).then(r => r.data),
    enabled: !!userId,
  });
  const { data: managerTasks = [] } = useQuery({
    queryKey: ['manager-tasks-dashboard', userId],
    queryFn: () => tasksApi.getByManager(userId!).then(r => r.data),
    enabled: !!userId,
  });

  const totalTasks = managerTasks.length;
  const completedTasks = managerTasks.filter((task) => task.type?.toLowerCase() === 'done').length;
  const workerTaskNamesMap = managerTasks.reduce((acc, task) => {
    if (!task.userId) return acc;
    if (!acc[task.userId]) acc[task.userId] = [];
    acc[task.userId].push(task.name);
    return acc;
  }, {} as Record<number, string[]>);

  const chartData = projects.map(p => ({
    name: p.name?.substring(0, 15),
    completed: p.completedTasks || 0,
    pending: (p.totalTasks || 0) - (p.completedTasks || 0),
  }));

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl text-display text-foreground">{t('לוח בקרה', 'Dashboard')}</h1>
        <p className="text-muted-foreground text-sm">{t('הפרויקטים שלי', 'My Projects')}</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <StatCard title={t('פרויקטים', 'Projects')} value={projects.length} icon={FolderKanban} color="accent" />
        <StatCard title={t('הושלמו', 'Completed')} value={completedTasks} icon={CheckCircle2} color="success" />
        <StatCard title={t('סה"כ משימות', 'Total Tasks')} value={totalTasks} icon={ListTodo} color="info" />
      </div>

      {chartData.length > 0 && (
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="bg-card rounded-xl border shadow-card p-5">
          <h3 className="text-sm font-semibold text-foreground mb-4">{t('משימות לפי פרויקט', 'Tasks by Project')}</h3>
          <ResponsiveContainer width="100%" height={240}>
            <BarChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" stroke="hsl(214, 32%, 91%)" />
              <XAxis dataKey="name" tick={{ fontSize: 11 }} stroke="hsl(215, 16%, 47%)" />
              <YAxis tick={{ fontSize: 12 }} stroke="hsl(215, 16%, 47%)" />
              <Tooltip />
              <Bar dataKey="completed" fill="hsl(262, 83%, 58%)" radius={[4, 4, 0, 0]} />
              <Bar dataKey="pending" fill="hsl(215, 16%, 80%)" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </motion.div>
      )}

      <div className="bg-card rounded-xl border shadow-card">
        <div className="p-5 border-b">
          <h3 className="text-sm font-semibold text-foreground">{t('הפרויקטים שלי', 'My Projects')}</h3>
        </div>
        <div className="divide-y">
          {projects.map((p) => (
            <div key={p.id} className="px-5 py-3 flex items-center justify-between transition-spring hover:bg-muted/50">
              <div>
                <p className="text-sm font-medium text-foreground">{p.name}</p>
                <p className="text-xs text-muted-foreground">{p.description}</p>
              </div>
              <StatusBadge status={p.status} />
            </div>
          ))}
          {projects.length === 0 && (
            <div className="p-8 text-center text-sm text-muted-foreground">{t('אין פרויקטים', 'No projects')}</div>
          )}
        </div>
      </div>

      <div className="bg-card rounded-xl border shadow-card">
        <div className="p-5 border-b">
          <h3 className="text-sm font-semibold text-foreground flex items-center gap-2">
            <Users className="h-4 w-4 text-accent" />
            {t('עובדים על המשימות שלי', 'Workers on my tasks')}
          </h3>
        </div>
        <div className="divide-y">
          {workers.map((w) => (
            <div key={w.id} className="px-5 py-3 flex items-center justify-between transition-spring hover:bg-muted/50">
              <div>
                <p className="text-sm font-medium text-foreground">{w.name}</p>
                <a className="text-xs text-muted-foreground hover:underline" href={`mailto:${w.email}`}>
                  {w.email}
                </a>
                <p className="text-xs text-muted-foreground mt-1">
                  {t('עובד על', 'Working on')}: {(workerTaskNamesMap[w.id] || []).slice(0, 2).join(', ') || t('ללא משימות פעילות', 'No active tasks')}
                </p>
              </div>
              <p className="text-xs text-muted-foreground">{t('ת.ז', 'ID')}: {w.tz}</p>
            </div>
          ))}
          {workers.length === 0 && (
            <div className="p-8 text-center text-sm text-muted-foreground">
              {t('אין עובדים משויכים כרגע למשימות שלך', 'No workers are currently assigned to your tasks')}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ManagerDashboard;
