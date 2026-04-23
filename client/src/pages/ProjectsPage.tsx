import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { useNavigate } from 'react-router-dom';
import { FolderKanban, Plus, Calendar, User, Pencil } from 'lucide-react';
import { projectsApi, tasksApi, usersApi } from '@/api/services';
import { useAuthStore } from '@/store/useAuthStore';
import { useLanguage } from '@/hooks/useLanguage';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import StatusBadge from '@/components/StatusBadge';
import ProjectDetailsDialog from '@/components/ProjectDetailsDialog';
import { toast } from 'sonner';

const ProjectsPage = () => {
  const { t, lang } = useLanguage();
  const { role, userId } = useAuthStore();
  const navigate = useNavigate();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [taskOpen, setTaskOpen] = useState(false);
  const [detailsProjectId, setDetailsProjectId] = useState<number | null>(null);
  const [form, setForm] = useState({
    name: '', description: '', startDate: '', dueDate: '', status: 'active', managerId: 0,
  });
  const [editForm, setEditForm] = useState({
    id: 0, name: '', description: '', startDate: '', dueDate: '', status: 'active', managerId: 0,
  });
  const [taskForm, setTaskForm] = useState({
    name: '',
    description: '',
    type: 'pending',
    priority: 3,
    projectId: 0,
    userId: 0,
  });

  const isHead = role === 'headmanager';
  const isManager = role === 'manager';
  const isHebrew = lang === 'he';
  const normalizeLevel = (level: string | number) => {
    if (typeof level === 'number') {
      if (level === 1) return 'manager';
      if (level === 2) return 'headmanager';
      return 'worker';
    }
    return level || 'worker';
  };

  const { data: projects = [] } = useQuery({
    queryKey: ['projects'],
    queryFn: () => isHead
      ? projectsApi.getAll().then(r => r.data)
      : projectsApi.getByManager(userId!).then(r => r.data),
  });

  const { data: projectTasksMap = {} } = useQuery({
    queryKey: ['project-tasks-map', projects.map(p => p.id).join(',')],
    queryFn: async () => {
      if (!projects.length) return {} as Record<number, any[]>;
      const entries = await Promise.all(
        projects.map(async (p) => {
          try {
            const res = await tasksApi.getByProject(p.id);
            return [p.id, res.data] as const;
          } catch {
            return [p.id, []] as const;
          }
        })
      );
      return Object.fromEntries(entries) as Record<number, any[]>;
    },
    enabled: projects.length > 0,
  });

  const { data: managers = [] } = useQuery({
    queryKey: ['users'],
    queryFn: () => usersApi.getAll().then(r => r.data),
    enabled: isHead,
  });

  const { data: workers = [] } = useQuery({
    queryKey: ['workers-for-projects', role, userId],
    queryFn: () => usersApi.getWorkers().then(r => r.data),
    enabled: isHead || (!!isManager && !!userId),
  });
  const workerNameById = workers.reduce((acc, w) => {
    acc[w.id] = w.name;
    return acc;
  }, {} as Record<number, string>);
  const managerNameById = managers.reduce((acc, m) => {
    acc[m.id] = m.name;
    return acc;
  }, {} as Record<number, string>);

  const createMutation = useMutation({
    mutationFn: (data: any) => projectsApi.create(data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['projects'] }); setOpen(false); toast.success(t('פרויקט נוצר', 'Project created')); },
    onError: () => toast.error(t('שגיאה', 'Error')),
  });

  const updateMutation = useMutation({
    mutationFn: (data: any) => projectsApi.update(data.id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['projects'] });
      setEditOpen(false);
      toast.success(t('הפרויקט עודכן', 'Project updated'));
    },
    onError: () => toast.error(t('שגיאה', 'Error')),
  });

  const createTaskMutation = useMutation({
    mutationFn: (data: any) => tasksApi.create(data),
    onSuccess: () => {
      setTaskOpen(false);
      qc.invalidateQueries({ queryKey: ['tasks'] });
      toast.success(t('המשימה נוספה לפרויקט', 'Task added to project'));
    },
    onError: () => toast.error(t('שגיאה', 'Error')),
  });

  const handleCreate = (e: React.FormEvent) => {
    e.preventDefault();
    if (isHead && form.managerId <= 0) {
      toast.error(t('יש לבחור מנהל פרויקט', 'Please select a project manager'));
      return;
    }
    createMutation.mutate(form);
  };

  const openEditDialog = (project: any) => {
    setEditForm({
      id: project.id,
      name: project.name || '',
      description: project.description || '',
      startDate: project.startDate?.slice?.(0, 10) || '',
      dueDate: project.dueDate?.slice?.(0, 10) || '',
      status: project.status || 'active',
      managerId: project.managerId || (isManager ? (userId || 0) : 0),
    });
    setEditOpen(true);
  };

  const handleUpdate = (e: React.FormEvent) => {
    e.preventDefault();
    const payload = {
      ...editForm,
      managerId: isManager ? (userId || editForm.managerId) : editForm.managerId,
    };
    if (isHead && payload.managerId <= 0) {
      toast.error(t('יש לבחור מנהל פרויקט', 'Please select a project manager'));
      return;
    }
    updateMutation.mutate(payload);
  };

  const currentManagerName = managers.find(m => m.id === editForm.managerId)?.name || '-';

  const openCreateTaskDialog = (projectId: number) => {
    setTaskForm({
      name: '',
      description: '',
      type: 'pending',
      priority: 3,
      projectId,
      userId: 0,
    });
    setTaskOpen(true);
  };

  const handleCreateTask = (e: React.FormEvent) => {
    e.preventDefault();
    if (taskForm.projectId <= 0) {
      toast.error(t('פרויקט לא תקין', 'Invalid project'));
      return;
    }
    if (taskForm.userId <= 0) {
      toast.error(t('יש לבחור עובד', 'Please select a worker'));
      return;
    }
    createTaskMutation.mutate(taskForm);
  };

  const detailsProject = projects.find((p) => p.id === detailsProjectId) || null;
  const detailsTasks = detailsProjectId ? (projectTasksMap[detailsProjectId] || []) : [];

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl text-display text-foreground">{t('פרויקטים', 'Projects')}</h1>
          <p className="text-muted-foreground text-sm">{projects.length} {t('פרויקטים', 'projects')}</p>
        </div>
        {isHead && (
          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="gradient-accent text-accent-foreground active-scale">
                <Plus className="h-4 w-4 me-1" /> {t('פרויקט חדש', 'New Project')}
              </Button>
            </DialogTrigger>
            <DialogContent dir={isHebrew ? 'rtl' : 'ltr'} className={isHebrew ? 'text-right' : ''}>
              <DialogHeader><DialogTitle>{t('פרויקט חדש', 'New Project')}</DialogTitle></DialogHeader>
              <form onSubmit={handleCreate} className="space-y-3">
                <div><Label>{t('שם', 'Name')}</Label><Input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required /></div>
                <div><Label>{t('תיאור', 'Description')}</Label><Textarea value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} /></div>
                <div className="grid grid-cols-2 gap-3">
                  <div><Label>{t('תאריך התחלה', 'Start Date')}</Label><Input type="date" value={form.startDate} onChange={e => setForm({ ...form, startDate: e.target.value })} required /></div>
                  <div><Label>{t('תאריך יעד', 'Due Date')}</Label><Input type="date" value={form.dueDate} onChange={e => setForm({ ...form, dueDate: e.target.value })} required /></div>
                </div>
                <div>
                  <Label>{t('מנהל פרויקט', 'Project Manager')}</Label>
                  <Select value={String(form.managerId)} onValueChange={v => setForm({ ...form, managerId: Number(v) })}>
                    <SelectTrigger><SelectValue placeholder={t('בחר מנהל', 'Select manager')} /></SelectTrigger>
                    <SelectContent>
                      {managers.filter(m => {
                        const level = normalizeLevel(m.level as unknown as string | number);
                        return level === 'manager' || level === 'headmanager';
                      }).map(m => (
                        <SelectItem key={m.id} value={String(m.id)}>{m.name}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label>{t('סטטוס', 'Status')}</Label>
                  <Select value={form.status} onValueChange={v => setForm({ ...form, status: v })}>
                    <SelectTrigger><SelectValue /></SelectTrigger>
                    <SelectContent>
                      <SelectItem value="active">{t('פעיל', 'Active')}</SelectItem>
                      <SelectItem value="in-progress">{t('בביצוע', 'In Progress')}</SelectItem>
                      <SelectItem value="done">{t('הושלם', 'Done')}</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <Button type="submit" className="w-full gradient-accent text-accent-foreground active-scale">{t('צור פרויקט', 'Create')}</Button>
              </form>
            </DialogContent>
          </Dialog>
        )}
      </div>

      {(isHead || isManager) && (
        <Dialog open={editOpen} onOpenChange={setEditOpen}>
          <DialogContent dir={isHebrew ? 'rtl' : 'ltr'} className={isHebrew ? 'text-right' : ''}>
            <DialogHeader><DialogTitle>{t('עריכת פרויקט', 'Edit Project')}</DialogTitle></DialogHeader>
            <form onSubmit={handleUpdate} className="space-y-3">
              <div><Label>{t('שם', 'Name')}</Label><Input value={editForm.name} onChange={e => setEditForm({ ...editForm, name: e.target.value })} required /></div>
              <div><Label>{t('תיאור', 'Description')}</Label><Textarea value={editForm.description} onChange={e => setEditForm({ ...editForm, description: e.target.value })} /></div>
              <div className="grid grid-cols-2 gap-3">
                <div><Label>{t('תאריך התחלה', 'Start Date')}</Label><Input type="date" value={editForm.startDate} onChange={e => setEditForm({ ...editForm, startDate: e.target.value })} required /></div>
                <div><Label>{t('תאריך יעד', 'Due Date')}</Label><Input type="date" value={editForm.dueDate} onChange={e => setEditForm({ ...editForm, dueDate: e.target.value })} required /></div>
              </div>
              <div>
                <Label>{t('סטטוס', 'Status')}</Label>
                <Select value={editForm.status} onValueChange={v => setEditForm({ ...editForm, status: v })}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="active">{t('פעיל', 'Active')}</SelectItem>
                    <SelectItem value="in-progress">{t('בביצוע', 'In Progress')}</SelectItem>
                    <SelectItem value="done">{t('הושלם', 'Done')}</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              {isHead && (
                <div>
                <Label>{t('מנהל נוכחי', 'Current Manager')}</Label>
                <div className="text-sm text-muted-foreground mb-2">{currentManagerName}</div>
                <Label>{t('מנהל פרויקט', 'Project Manager')}</Label>
                <Select value={String(editForm.managerId)} onValueChange={v => setEditForm({ ...editForm, managerId: Number(v) })}>
                  <SelectTrigger><SelectValue placeholder={t('בחר מנהל', 'Select manager')} /></SelectTrigger>
                  <SelectContent>
                    {managers.filter(m => {
                      const level = normalizeLevel(m.level as unknown as string | number);
                      return level === 'manager' || level === 'headmanager';
                    }).map(m => (
                      <SelectItem key={m.id} value={String(m.id)}>{m.name}</SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              )}
              <Button type="submit" className="w-full gradient-accent text-accent-foreground active-scale">{t('שמור שינויים', 'Save changes')}</Button>
            </form>
          </DialogContent>
        </Dialog>
      )}

      {(isHead || isManager) && (
        <Dialog open={taskOpen} onOpenChange={setTaskOpen}>
          <DialogContent dir={isHebrew ? 'rtl' : 'ltr'} className={isHebrew ? 'text-right' : ''}>
            <DialogHeader><DialogTitle>{t('הוספת משימה לפרויקט', 'Add Task To Project')}</DialogTitle></DialogHeader>
            <form onSubmit={handleCreateTask} className="space-y-3">
              <div><Label>{t('שם', 'Name')}</Label><Input value={taskForm.name} onChange={e => setTaskForm({ ...taskForm, name: e.target.value })} required /></div>
              <div><Label>{t('תיאור', 'Description')}</Label><Textarea value={taskForm.description} onChange={e => setTaskForm({ ...taskForm, description: e.target.value })} /></div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <Label>{t('עדיפות', 'Priority')}</Label>
                  <Input type="number" min={1} max={5} value={taskForm.priority} onChange={e => setTaskForm({ ...taskForm, priority: Number(e.target.value) })} />
                </div>
                <div>
                  <Label>{t('עובד', 'Worker')}</Label>
                  <Select value={String(taskForm.userId)} onValueChange={v => setTaskForm({ ...taskForm, userId: Number(v) })}>
                    <SelectTrigger><SelectValue placeholder={t('בחר עובד', 'Select worker')} /></SelectTrigger>
                    <SelectContent>
                      {workers.filter(w => normalizeLevel(w.level as unknown as string | number) === 'worker').length === 0 && (
                        <SelectItem value="__none" disabled>{t('אין עובדים זמינים', 'No workers available')}</SelectItem>
                      )}
                      {workers
                        .filter(w => normalizeLevel(w.level as unknown as string | number) === 'worker')
                        .map(w => <SelectItem key={w.id} value={String(w.id)}>{w.name}</SelectItem>)}
                    </SelectContent>
                  </Select>
                </div>
              </div>
              <Button type="submit" className="w-full gradient-accent text-accent-foreground active-scale">{t('הוסף משימה', 'Add Task')}</Button>
            </form>
          </DialogContent>
        </Dialog>
      )}

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {projects.map((p, i) => (
          <motion.div
            key={p.id}
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: i * 0.05 }}
            className="bg-card rounded-xl border shadow-card p-5 transition-spring hover:shadow-elevated cursor-pointer"
            onClick={() => setDetailsProjectId(p.id)}
          >
            <div className="flex items-start justify-between mb-3">
              <div className="flex items-center gap-2">
                <div className="p-1.5 rounded-lg bg-accent/10">
                  <FolderKanban className="h-4 w-4 text-accent" />
                </div>
                <h3 className="text-sm font-semibold text-foreground">{p.name}</h3>
              </div>
              <div className="flex items-center gap-2">
                <StatusBadge status={p.status} />
                {(isHead || isManager) && (
                  <>
                    <Button
                      variant="ghost"
                      size="icon"
                      className="h-7 w-7"
                      onClick={(e) => {
                        e.stopPropagation();
                        openEditDialog(p);
                      }}
                    >
                      <Pencil className="h-3.5 w-3.5" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-7 px-2 text-xs"
                      onClick={(e) => {
                        e.stopPropagation();
                        openCreateTaskDialog(p.id);
                      }}
                    >
                      <Plus className="h-3 w-3 me-1" />
                      {t('משימה', 'Task')}
                    </Button>
                  </>
                )}
              </div>
            </div>
            <p className="text-xs text-muted-foreground mb-3 line-clamp-2">{p.description}</p>
            <div className="flex items-center gap-3 text-[10px] text-muted-foreground/60">
              <span className="flex items-center gap-1"><Calendar className="h-3 w-3" />{new Date(p.dueDate).toLocaleDateString()}</span>
              {p.managerName && <span className="flex items-center gap-1"><User className="h-3 w-3" />{p.managerName}</span>}
            </div>
            <div className="mt-3">
              <p className="text-[11px] font-medium text-foreground mb-1">
                {t('משימות בפרויקט', 'Project tasks')}
              </p>
              <div className="space-y-1">
                {(projectTasksMap[p.id] || []).slice(0, 5).map((task) => (
                  <div key={task.id} className="text-[11px] text-muted-foreground bg-muted/40 rounded px-2 py-1">
                    <div className="font-medium text-foreground/90">{task.name}</div>
                    <div className="mt-0.5 flex items-center gap-2 text-[10px] text-muted-foreground/80">
                      <StatusBadge status={task.type || 'pending'} />
                      <span>
                        {t('עובד', 'Worker')}: {task.userName || workerNameById[task.userId] || '-'}
                      </span>
                    </div>
                  </div>
                ))}
                {(projectTasksMap[p.id] || []).length === 0 && (
                  <p className="text-[11px] text-muted-foreground/70">
                    {t('אין משימות לפרויקט זה', 'No tasks for this project')}
                  </p>
                )}
              </div>
            </div>
            {p.totalTasks !== undefined && p.totalTasks > 0 && (
              <div className="mt-3">
                <div className="flex justify-between text-[10px] text-muted-foreground mb-1">
                  <span>{t('התקדמות', 'Progress')}</span>
                  <span>{Math.round(p.completionPercentage || 0)}%</span>
                </div>
                <div className="h-1.5 rounded-full bg-muted overflow-hidden">
                  <div
                    className="h-full rounded-full gradient-accent transition-all"
                    style={{ width: `${p.completionPercentage || 0}%` }}
                  />
                </div>
              </div>
            )}
          </motion.div>
        ))}
      </div>

      {projects.length === 0 && (
        <div className="bg-card rounded-xl border shadow-card p-12 text-center">
          <FolderKanban className="h-10 w-10 text-muted-foreground/30 mx-auto mb-3" />
          <p className="text-sm text-muted-foreground">{t('אין פרויקטים', 'No projects')}</p>
        </div>
      )}

      <ProjectDetailsDialog
        open={!!detailsProject}
        onOpenChange={(open) => {
          if (!open) setDetailsProjectId(null);
        }}
        project={detailsProject}
        tasks={detailsTasks}
        managerName={detailsProject ? (detailsProject.managerName || managerNameById[detailsProject.managerId]) : undefined}
        t={t}
        isHebrew={isHebrew}
        onViewTasks={(projectId) => navigate(`${isHead ? '/tasks' : '/manager/tasks'}?projectId=${projectId}`)}
      />
    </div>
  );
};

export default ProjectsPage;
