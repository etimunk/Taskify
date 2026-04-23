import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ListTodo, CheckCircle2, Clock, AlertTriangle, ArrowUpDown } from 'lucide-react';
import { useAuthStore } from '@/store/useAuthStore';
import { tasksApi } from '@/api/services';
import { useLanguage } from '@/hooks/useLanguage';
import StatCard from '@/components/StatCard';
import StatusBadge from '@/components/StatusBadge';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { toast } from 'sonner';
import type { Task } from '@/types';
import { useState } from 'react';

const WorkerDashboard = () => {
  const formatDate = (dateValue?: string) => {
    if (!dateValue) return '-';
    const date = new Date(dateValue);
    if (Number.isNaN(date.getTime())) return '-';
    return date.toLocaleDateString(undefined, { timeZone: 'UTC' });
  };

  const { t } = useLanguage();
  const { userId } = useAuthStore();
  const qc = useQueryClient();
  const [priorityFilter, setPriorityFilter] = useState('all');

  const { data: tasks = [] } = useQuery({
    queryKey: ['worker-tasks', userId],
    queryFn: async () => {
      const r = await tasksApi.getByWorker(userId!);
      return (r.data as unknown as Record<string, unknown>[]).map((raw) => ({
        ...(raw as object),
        id: (raw as any).id ?? (raw as any).Id,
        name: (raw as any).name ?? (raw as any).Name,
        description: (raw as any).description ?? (raw as any).Description,
        type: (raw as any).type ?? (raw as any).Type,
        priority: (raw as any).priority ?? (raw as any).Priority,
        projectId: (raw as any).projectId ?? (raw as any).ProjectId,
        userId: (raw as any).userId ?? (raw as any).UserId,
        userName: (raw as any).userName ?? (raw as any).UserName,
        projectName: (raw as any).projectName ?? (raw as any).ProjectName,
        projectDueDate: (raw as any).projectDueDate ?? (raw as any).ProjectDueDate,
        projectManagerName: (raw as any).projectManagerName ?? (raw as any).ProjectManagerName,
        projectManagerEmail: (raw as any).projectManagerEmail ?? (raw as any).ProjectManagerEmail,
      })) as Task[];
    },
    enabled: !!userId,
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, task }: { id: number; task: any }) => tasksApi.update(id, task),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['worker-tasks'] });
      toast.success(t('המשימה עודכנה', 'Task updated'));
    },
    onError: () => toast.error(t('שגיאה בעדכון', 'Update failed')),
  });

  const handleStatusChange = (task: Task, newType: string) => {
    updateMutation.mutate({
      id: task.id,
      task: { ...task, type: newType },
    });
  };

  const done = tasks.filter(t => t.type?.toLowerCase() === 'done').length;
  const inProgress = tasks.filter(t => t.type?.toLowerCase() === 'in-progress').length;
  const pending = tasks.filter((t) => {
    const status = t.type?.toLowerCase();
    return status !== 'in-progress' && status !== 'done';
  }).length;

  const priorityFilteredTasks = priorityFilter === 'all'
    ? tasks
    : tasks.filter((task) => String(task.priority) === priorityFilter);

  const groupedByProject = Object.values(
    priorityFilteredTasks.reduce((acc, task) => {
      const projectKey = String(task.projectId);
      if (!acc[projectKey]) {
        acc[projectKey] = {
          projectId: task.projectId,
          projectName: task.projectName || t('פרויקט ללא שם', 'Unnamed project'),
          projectManagerName: task.projectManagerName || '-',
          projectManagerEmail: task.projectManagerEmail,
          projectDueDate: task.projectDueDate,
          tasks: [],
        };
      }
      if (task.projectName && acc[projectKey].projectName === t('פרויקט ללא שם', 'Unnamed project')) {
        acc[projectKey].projectName = task.projectName;
      }
      if (task.projectManagerName && acc[projectKey].projectManagerName === '-') {
        acc[projectKey].projectManagerName = task.projectManagerName;
      }
      if (task.projectDueDate && !acc[projectKey].projectDueDate) {
        acc[projectKey].projectDueDate = task.projectDueDate;
      }
      if (task.projectManagerEmail && !acc[projectKey].projectManagerEmail) {
        acc[projectKey].projectManagerEmail = task.projectManagerEmail;
      }
      acc[projectKey].tasks.push(task);
      return acc;
    }, {} as Record<string, { projectId: number; projectName: string; projectManagerName: string; projectManagerEmail?: string; projectDueDate?: string; tasks: Task[] }>)
  ).map((group) => ({
    ...group,
    tasks: [...group.tasks].sort((a, b) => b.priority - a.priority),
  }));

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl text-display text-foreground">{t('המשימות שלי', 'My Tasks')}</h1>
        <p className="text-muted-foreground text-sm">{t('מה בתכנית להיום?', "What's on today?")}</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <StatCard title={t('הושלמו', 'Done')} value={done} icon={CheckCircle2} color="success" />
        <StatCard title={t('בביצוע', 'In Progress')} value={inProgress} icon={Clock} color="info" />
        <StatCard title={t('ממתינות', 'Pending')} value={pending} icon={AlertTriangle} color="warning" />
      </div>

      <div className="flex items-center gap-2">
        <p className="text-sm text-muted-foreground">{t('סינון לפי דחיפות', 'Filter by priority')}</p>
        <Select value={priorityFilter} onValueChange={setPriorityFilter}>
          <SelectTrigger className="w-36 h-8 text-xs">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">{t('הכל', 'All')}</SelectItem>
            <SelectItem value="1">1</SelectItem>
            <SelectItem value="2">2</SelectItem>
            <SelectItem value="3">3</SelectItem>
            <SelectItem value="4">4</SelectItem>
            <SelectItem value="5">5</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-4">
        {groupedByProject.map((group, groupIndex) => (
          <motion.div
            key={`${group.projectId}-${group.projectName}`}
            initial={{ opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: groupIndex * 0.04 }}
            className="bg-card rounded-xl border shadow-card overflow-hidden"
          >
            <div className="px-4 py-3 border-b bg-muted/20">
              <p className="text-sm font-semibold text-foreground">{group.projectName}</p>
              <div className="flex items-center gap-3 mt-1.5">
                <span className="text-[11px] text-muted-foreground">
                  {t('מנהל פרויקט', 'Project manager')}: {group.projectManagerName}
                </span>
                {group.projectManagerEmail && (
                  <a
                    className="text-[11px] text-accent underline-offset-2 hover:underline"
                    href={`mailto:${group.projectManagerEmail}`}
                  >
                    {group.projectManagerEmail}
                  </a>
                )}
                {group.projectDueDate && (
                  <span className="text-[11px] text-muted-foreground">
                    {t('סיום פרויקט', 'Project due')}: {formatDate(group.projectDueDate)}
                  </span>
                )}
              </div>
            </div>

            <div className="divide-y">
              {group.tasks.map((task) => (
                <div key={task.id} className="p-4 flex items-center justify-between gap-4 transition-spring hover:bg-muted/20">
                  <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2">
                      <p className="text-sm font-medium text-foreground">{task.name}</p>
                      <StatusBadge status={task.type} />
                    </div>
                    <p className="text-xs text-muted-foreground mt-0.5 truncate">{task.description}</p>
                    <div className="flex items-center gap-3 mt-1.5">
                      <span className="text-[10px] text-muted-foreground/60 flex items-center gap-1">
                        <ArrowUpDown className="h-3 w-3" /> {t('עדיפות', 'Priority')}: {task.priority}
                      </span>
                    </div>
                  </div>

                  <Select
                    value={task.type?.toLowerCase()}
                    onValueChange={(val) => handleStatusChange(task, val)}
                  >
                    <SelectTrigger className="w-32 h-8 text-xs">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="pending">{t('ממתין', 'Pending')}</SelectItem>
                      <SelectItem value="in-progress">{t('בביצוע', 'In Progress')}</SelectItem>
                      <SelectItem value="done">{t('הושלם', 'Done')}</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
              ))}
            </div>
          </motion.div>
        ))}

        {priorityFilteredTasks.length === 0 && (
          <div className="bg-card rounded-xl border shadow-card p-12 text-center">
            <ListTodo className="h-10 w-10 text-muted-foreground/30 mx-auto mb-3" />
            <p className="text-sm text-muted-foreground">{t('אין משימות כרגע', 'No tasks yet')}</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default WorkerDashboard;
