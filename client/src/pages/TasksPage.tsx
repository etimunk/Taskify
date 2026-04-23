import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ListTodo, Plus, ArrowUpDown } from 'lucide-react';
import { useSearchParams } from 'react-router-dom';
import { tasksApi, projectsApi, usersApi } from '@/api/services';
import { useAuthStore } from '@/store/useAuthStore';
import { useLanguage } from '@/hooks/useLanguage';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import StatusBadge from '@/components/StatusBadge';
import { toast } from 'sonner';
import type { Task } from '@/types';

const TasksPage = () => {
  const { t, lang } = useLanguage();
  const { role, userId } = useAuthStore();
  const qc = useQueryClient();
  const [searchParams, setSearchParams] = useSearchParams();
  const [open, setOpen] = useState(false);
  const [detailsOpen, setDetailsOpen] = useState(false);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [statusFilter, setStatusFilter] = useState('all');
  const [form, setForm] = useState({
    name: '', description: '', type: 'pending', priority: 3, projectId: 0, userId: 0,
  });

  const isHead = role === 'headmanager';
  const isManager = role === 'manager';
  const isHebrew = lang === 'he';
  const projectFilterId = Number(searchParams.get('projectId') || 0);
  const normalizeLevel = (level: string | number) => {
    if (typeof level === 'number') {
      if (level === 1) return 'manager';
      if (level === 2) return 'headmanager';
      return 'worker';
    }
    return level || 'worker';
  };

  const { data: tasks = [] } = useQuery({
    queryKey: ['tasks', role, userId],
    queryFn: () => isHead
      ? tasksApi.getAll().then(r => r.data)
      : tasksApi.getByManager(userId!).then(r => r.data),
    enabled: isHead || (!!isManager && !!userId),
  });

  const { data: projects = [] } = useQuery({
    queryKey: ['projects-for-tasks'],
    queryFn: () => isHead ? projectsApi.getAll().then(r => r.data) : projectsApi.getByManager(userId!).then(r => r.data),
    enabled: isHead || isManager,
  });

  const { data: users = [] } = useQuery({
    queryKey: ['users-for-tasks'],
    queryFn: () => usersApi.getWorkers().then(r => r.data),
    enabled: isHead || (!!isManager && !!userId),
  });
  const projectNameById = projects.reduce((acc, p) => {
    acc[p.id] = p.name;
    return acc;
  }, {} as Record<number, string>);
  const userNameById = users.reduce((acc, u) => {
    acc[u.id] = u.name;
    return acc;
  }, {} as Record<number, string>);

  const createMutation = useMutation({
    mutationFn: (data: any) => tasksApi.create(data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['tasks'] }); setOpen(false); toast.success(t('משימה נוצרה', 'Task created')); },
    onError: () => toast.error(t('שגיאה', 'Error')),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, task }: { id: number; task: any }) => tasksApi.update(id, task),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['tasks'] }); toast.success(t('עודכן', 'Updated')); },
  });

  const handleCreate = (e: React.FormEvent) => {
    e.preventDefault();
    if (form.projectId <= 0) {
      toast.error(t('יש לבחור פרויקט', 'Please select a project'));
      return;
    }
    if (form.userId <= 0) {
      toast.error(t('יש לבחור עובד', 'Please select a worker'));
      return;
    }
    createMutation.mutate(form);
  };

  const handleStatusChange = (task: Task, newType: string) => {
    updateMutation.mutate({ id: task.id, task: { ...task, type: newType } });
  };

  const statusFiltered = statusFilter === 'all' ? tasks : tasks.filter(t => t.type?.toLowerCase() === statusFilter);
  const filtered = projectFilterId > 0
    ? statusFiltered.filter((t) => t.projectId === projectFilterId)
    : statusFiltered;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl text-display text-foreground">{t('משימות', 'Tasks')}</h1>
          <p className="text-muted-foreground text-sm">{filtered.length} {t('משימות', 'tasks')}</p>
        </div>
        <div className="flex gap-2">
          <Select value={statusFilter} onValueChange={setStatusFilter}>
            <SelectTrigger className="h-9 w-32 text-xs">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">{t('הכל', 'All')}</SelectItem>
              <SelectItem value="pending">{t('ממתין', 'Pending')}</SelectItem>
              <SelectItem value="in-progress">{t('בביצוע', 'In Progress')}</SelectItem>
              <SelectItem value="done">{t('הושלם', 'Done')}</SelectItem>
            </SelectContent>
          </Select>
          {projectFilterId > 0 && (
            <Button
              variant="outline"
              className="h-9 text-xs"
              onClick={() => {
                searchParams.delete('projectId');
                setSearchParams(searchParams);
              }}
            >
              {t('נקה סינון פרויקט', 'Clear project filter')}
            </Button>
          )}

          {isManager && (
            <Dialog open={open} onOpenChange={setOpen}>
              <DialogTrigger asChild>
                <Button className="gradient-accent text-accent-foreground active-scale h-9">
                  <Plus className="h-4 w-4 me-1" /> {t('משימה חדשה', 'New Task')}
                </Button>
              </DialogTrigger>
              <DialogContent dir={isHebrew ? 'rtl' : 'ltr'} className={isHebrew ? 'text-right' : ''}>
                <DialogHeader><DialogTitle>{t('משימה חדשה', 'New Task')}</DialogTitle></DialogHeader>
                <form onSubmit={handleCreate} className="space-y-3">
                  <div><Label>{t('שם', 'Name')}</Label><Input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required /></div>
                  <div><Label>{t('תיאור', 'Description')}</Label><Textarea value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} /></div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <Label>{t('פרויקט', 'Project')}</Label>
                      <Select value={String(form.projectId)} onValueChange={v => setForm({ ...form, projectId: Number(v) })}>
                        <SelectTrigger><SelectValue placeholder={t('בחר', 'Select')} /></SelectTrigger>
                        <SelectContent>{projects.map(p => <SelectItem key={p.id} value={String(p.id)}>{p.name}</SelectItem>)}</SelectContent>
                      </Select>
                    </div>
                    <div>
                      <Label>{t('עובד', 'Worker')}</Label>
                      <Select value={String(form.userId)} onValueChange={v => setForm({ ...form, userId: Number(v) })}>
                        <SelectTrigger><SelectValue placeholder={t('בחר', 'Select')} /></SelectTrigger>
                        <SelectContent>
                          {users.filter(u => normalizeLevel(u.level as unknown as string | number) === 'worker').length === 0 && (
                            <SelectItem value="__none" disabled>{t('אין עובדים זמינים', 'No workers available')}</SelectItem>
                          )}
                          {users.filter(u => normalizeLevel(u.level as unknown as string | number) === 'worker').map(u => <SelectItem key={u.id} value={String(u.id)}>{u.name}</SelectItem>)}
                        </SelectContent>
                      </Select>
                    </div>
                  </div>
                  <div>
                    <Label>{t('עדיפות', 'Priority')} (1-5)</Label>
                    <Input type="number" min={1} max={5} value={form.priority} onChange={e => setForm({ ...form, priority: Number(e.target.value) })} />
                  </div>
                  <Button type="submit" className="w-full gradient-accent text-accent-foreground active-scale">{t('צור', 'Create')}</Button>
                </form>
              </DialogContent>
            </Dialog>
          )}
        </div>
      </div>

      <div className="bg-card rounded-xl border shadow-card overflow-hidden">
        <div className="divide-y">
          {filtered.map((task, i) => (
            <motion.div
              key={task.id}
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ delay: i * 0.02 }}
              className="px-5 py-3.5 flex items-center justify-between gap-4 transition-spring hover:bg-muted/30 cursor-pointer"
              onClick={() => {
                setSelectedTask(task);
                setDetailsOpen(true);
              }}
            >
              <div className="min-w-0 flex-1">
                <div className="flex items-center gap-2">
                  <p className="text-sm font-medium text-foreground">{task.name}</p>
                  <span className="text-[10px] text-muted-foreground/60 flex items-center gap-0.5">
                    <ArrowUpDown className="h-2.5 w-2.5" />{task.priority}
                  </span>
                </div>
                <p className="text-xs text-muted-foreground truncate">{task.description}</p>
              </div>

              <div className="flex items-center gap-2 shrink-0" onClick={(e) => e.stopPropagation()}>
                {isManager ? (
                  <Select
                    value={task.type?.toLowerCase()}
                    onValueChange={(val) => handleStatusChange(task, val)}
                  >
                    <SelectTrigger className="h-7 text-xs w-28"><SelectValue /></SelectTrigger>
                    <SelectContent>
                      <SelectItem value="pending">{t('ממתין', 'Pending')}</SelectItem>
                      <SelectItem value="in-progress">{t('בביצוע', 'In Progress')}</SelectItem>
                      <SelectItem value="done">{t('הושלם', 'Done')}</SelectItem>
                    </SelectContent>
                  </Select>
                ) : (
                  <StatusBadge status={task.type} />
                )}
              </div>
            </motion.div>
          ))}
          {filtered.length === 0 && (
            <div className="p-12 text-center text-sm text-muted-foreground">
              <ListTodo className="h-8 w-8 text-muted-foreground/30 mx-auto mb-2" />
              {t('אין משימות', 'No tasks')}
            </div>
          )}
        </div>
      </div>

      <Dialog open={detailsOpen} onOpenChange={setDetailsOpen}>
        <DialogContent dir={isHebrew ? 'rtl' : 'ltr'} className={isHebrew ? 'text-right' : ''}>
          <DialogHeader>
            <DialogTitle>{selectedTask?.name || t('פרטי משימה', 'Task Details')}</DialogTitle>
          </DialogHeader>
          {selectedTask && (
            <div className="space-y-3 text-sm">
              <div className="flex items-center gap-2">
                <StatusBadge status={selectedTask.type} />
                <span className="text-muted-foreground">{t('עדיפות', 'Priority')}: {selectedTask.priority}</span>
              </div>
              <div>
                <p className="text-xs text-muted-foreground">{t('תיאור', 'Description')}</p>
                <p>{selectedTask.description || '-'}</p>
              </div>
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                <p><span className="text-muted-foreground">{t('פרויקט', 'Project')}:</span> {selectedTask.projectName || projectNameById[selectedTask.projectId] || '-'}</p>
                <p><span className="text-muted-foreground">{t('עובד', 'Worker')}:</span> {selectedTask.userName || userNameById[selectedTask.userId] || '-'}</p>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default TasksPage;
