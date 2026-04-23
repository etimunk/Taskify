import { useMemo, useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { FolderKanban, Users, ListTodo, TrendingUp, CheckCircle2, Clock, AlertTriangle } from 'lucide-react';
import { usersApi, projectsApi, tasksApi } from '@/api/services';
import { useLanguage } from '@/hooks/useLanguage';
import StatCard from '@/components/StatCard';
import StatusBadge from '@/components/StatusBadge';
import ProjectDetailsDialog from '@/components/ProjectDetailsDialog';

const CHART_COLORS = ['hsl(262, 83%, 58%)', 'hsl(142, 76%, 36%)', 'hsl(38, 92%, 50%)', 'hsl(199, 89%, 48%)'];

const HeadManagerDashboard = () => {
  const { t } = useLanguage();
  const navigate = useNavigate();
  const [selectedProjectId, setSelectedProjectId] = useState<number | null>(null);

  const { data: users = [] } = useQuery({ queryKey: ['users'], queryFn: () => usersApi.getAll().then(r => r.data) });
  const { data: projects = [] } = useQuery({ queryKey: ['projects'], queryFn: () => projectsApi.getAll().then(r => r.data) });
  const { data: tasks = [] } = useQuery({ queryKey: ['tasks'], queryFn: () => tasksApi.getAll().then(r => r.data) });

  const completedTasks = tasks.filter(t => t.type?.toLowerCase() === 'done').length;
  const inProgressTasks = tasks.filter(t => t.type?.toLowerCase() === 'in-progress').length;
  const pendingTasks = tasks.filter(t => t.type?.toLowerCase() === 'pending').length;

  const pieData = [
    { name: t('הושלם', 'Done'), value: completedTasks },
    { name: t('בביצוע', 'In Progress'), value: inProgressTasks },
    { name: t('ממתין', 'Pending'), value: pendingTasks },
  ].filter(d => d.value > 0);

  // Mock trend data based on tasks
  const trendData = [
    { name: t('שבוע 1', 'Week 1'), completed: Math.round(completedTasks * 0.2), pending: Math.round(pendingTasks * 0.8) },
    { name: t('שבוע 2', 'Week 2'), completed: Math.round(completedTasks * 0.4), pending: Math.round(pendingTasks * 0.6) },
    { name: t('שבוע 3', 'Week 3'), completed: Math.round(completedTasks * 0.7), pending: Math.round(pendingTasks * 0.4) },
    { name: t('שבוע 4', 'Week 4'), completed: completedTasks, pending: pendingTasks },
  ];

  const selectedProject = useMemo(
    () => projects.find((p) => p.id === selectedProjectId) || null,
    [projects, selectedProjectId]
  );
  const userNameById = useMemo(
    () => users.reduce((acc, user) => {
      acc[user.id] = user.name;
      return acc;
    }, {} as Record<number, string>),
    [users]
  );
  const selectedProjectTasks = useMemo(
    () => tasks
      .filter((task) => task.projectId === selectedProjectId)
      .map((task) => ({
        ...task,
        userName: task.userName || userNameById[task.userId] || '-',
      })),
    [tasks, selectedProjectId, userNameById]
  );
  const selectedProjectManagerName = useMemo(() => {
    if (!selectedProject) return undefined;
    return selectedProject.managerName || userNameById[selectedProject.managerId];
  }, [selectedProject, userNameById]);

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl text-display text-foreground">{t('לוח בקרה', 'Dashboard')}</h1>
        <p className="text-muted-foreground text-sm">{t('מבט כולל על המערכת', 'System overview')}</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard title={t('משתמשים', 'Users')} value={users.length} icon={Users} color="accent" />
        <StatCard title={t('פרויקטים', 'Projects')} value={projects.length} icon={FolderKanban} color="info" />
        <StatCard title={t('משימות הושלמו', 'Completed')} value={completedTasks} icon={CheckCircle2} color="success" />
        <StatCard title={t('ממתינות', 'Pending')} value={pendingTasks} icon={Clock} color="warning" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        {/* Area Chart */}
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
          className="lg:col-span-2 bg-card rounded-xl border shadow-card p-5"
        >
          <h3 className="text-sm font-semibold text-foreground mb-4 flex items-center gap-2">
            <TrendingUp className="h-4 w-4 text-accent" />
            {t('קצב התקדמות', 'Project Pulse')}
          </h3>
          <ResponsiveContainer width="100%" height={240}>
            <AreaChart data={trendData}>
              <defs>
                <linearGradient id="colorCompleted" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="hsl(262, 83%, 58%)" stopOpacity={0.3} />
                  <stop offset="95%" stopColor="hsl(262, 83%, 58%)" stopOpacity={0} />
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" stroke="hsl(214, 32%, 91%)" />
              <XAxis dataKey="name" tick={{ fontSize: 12 }} stroke="hsl(215, 16%, 47%)" />
              <YAxis tick={{ fontSize: 12 }} stroke="hsl(215, 16%, 47%)" />
              <Tooltip />
              <Area type="monotone" dataKey="completed" stroke="hsl(262, 83%, 58%)" fill="url(#colorCompleted)" strokeWidth={2} />
              <Area type="monotone" dataKey="pending" stroke="hsl(215, 16%, 80%)" fill="hsl(215, 16%, 80%)" fillOpacity={0.1} strokeWidth={2} />
            </AreaChart>
          </ResponsiveContainer>
        </motion.div>

        {/* Pie Chart */}
        <motion.div
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
          className="bg-card rounded-xl border shadow-card p-5"
        >
          <h3 className="text-sm font-semibold text-foreground mb-4">
            {t('התפלגות משימות', 'Task Distribution')}
          </h3>
          {pieData.length > 0 ? (
            <ResponsiveContainer width="100%" height={200}>
              <PieChart>
                <Pie data={pieData} cx="50%" cy="50%" innerRadius={50} outerRadius={80} dataKey="value" paddingAngle={4}>
                  {pieData.map((_, i) => (
                    <Cell key={i} fill={CHART_COLORS[i % CHART_COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          ) : (
            <div className="flex items-center justify-center h-[200px] text-muted-foreground text-sm">
              {t('אין נתונים', 'No data')}
            </div>
          )}
          <div className="flex gap-4 justify-center mt-2">
            {pieData.map((d, i) => (
              <div key={d.name} className="flex items-center gap-1.5 text-xs">
                <div className="w-2.5 h-2.5 rounded-full" style={{ backgroundColor: CHART_COLORS[i] }} />
                <span className="text-muted-foreground">{d.name} ({d.value})</span>
              </div>
            ))}
          </div>
        </motion.div>
      </div>

      {/* Recent Projects */}
      <motion.div
        initial={{ opacity: 0, y: 10 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ delay: 0.3 }}
        className="bg-card rounded-xl border shadow-card"
      >
        <div className="p-5 border-b">
          <h3 className="text-sm font-semibold text-foreground flex items-center gap-2">
            <FolderKanban className="h-4 w-4 text-accent" />
            {t('פרויקטים אחרונים', 'Recent Projects')}
          </h3>
        </div>
        <div className="divide-y">
          {projects.slice(0, 5).map((project) => (
            <button
              key={project.id}
              type="button"
              onClick={() => setSelectedProjectId(project.id)}
              className="w-full text-left px-5 py-3 flex items-center justify-between transition-spring hover:bg-muted/50"
            >
              <div>
                <p className="text-sm font-medium text-foreground">{project.name}</p>
                <p className="text-xs text-muted-foreground truncate max-w-xs">{project.description}</p>
              </div>
              <StatusBadge status={project.status} />
            </button>
          ))}
          {projects.length === 0 && (
            <div className="p-8 text-center text-sm text-muted-foreground">
              {t('אין פרויקטים עדיין', 'No projects yet')}
            </div>
          )}
        </div>
      </motion.div>

      <ProjectDetailsDialog
        open={!!selectedProject}
        onOpenChange={(open) => {
          if (!open) setSelectedProjectId(null);
        }}
        project={selectedProject}
        tasks={selectedProjectTasks}
        managerName={selectedProjectManagerName}
        t={t}
        onViewTasks={(projectId) => navigate(`/tasks?projectId=${projectId}`)}
      />
    </div>
  );
};

export default HeadManagerDashboard;
