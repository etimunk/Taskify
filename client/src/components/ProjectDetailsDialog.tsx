import { Calendar, ListTodo, User } from 'lucide-react';
import type { Project, Task } from '@/types';
import StatusBadge from '@/components/StatusBadge';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';

interface ProjectDetailsDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  project: Project | null;
  tasks: Task[];
  managerName?: string;
  t: (he: string, en: string) => string;
  isHebrew?: boolean;
  onViewTasks?: (projectId: number) => void;
}

const ProjectDetailsDialog = ({
  open,
  onOpenChange,
  project,
  tasks,
  managerName,
  t,
  isHebrew = false,
  onViewTasks,
}: ProjectDetailsDialogProps) => {
  if (!project) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent dir={isHebrew ? 'rtl' : 'ltr'} className={`max-w-3xl max-h-[85vh] overflow-y-auto ${isHebrew ? 'text-right' : ''}`}>
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            {project.name}
            <StatusBadge status={project.status} />
          </DialogTitle>
          {onViewTasks && (
            <div className="pt-2">
              <Button size="sm" variant="outline" onClick={() => onViewTasks(project.id)}>
                {t('מעבר למשימות הפרויקט', 'Go to project tasks')}
              </Button>
            </div>
          )}
        </DialogHeader>

        <div className="space-y-4">
          <p className="text-sm text-muted-foreground">{project.description || '-'}</p>

          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
            <div className="rounded-lg border p-3">
              <p className="text-xs text-muted-foreground">{t('מנהל פרויקט', 'Project Manager')}</p>
              <p className="text-sm font-medium mt-1 flex items-center gap-1">
                <User className="h-3.5 w-3.5" /> {managerName || project.managerName || '-'}
              </p>
            </div>
            <div className="rounded-lg border p-3">
              <p className="text-xs text-muted-foreground">{t('תאריך התחלה', 'Start Date')}</p>
              <p className="text-sm font-medium mt-1 flex items-center gap-1">
                <Calendar className="h-3.5 w-3.5" /> {new Date(project.startDate).toLocaleDateString()}
              </p>
            </div>
            <div className="rounded-lg border p-3">
              <p className="text-xs text-muted-foreground">{t('תאריך סיום', 'Due Date')}</p>
              <p className="text-sm font-medium mt-1 flex items-center gap-1">
                <Calendar className="h-3.5 w-3.5" /> {new Date(project.dueDate).toLocaleDateString()}
              </p>
            </div>
          </div>

          <div>
            <h4 className="text-sm font-semibold mb-2 flex items-center gap-1">
              <ListTodo className="h-4 w-4" /> {t('משימות הפרויקט', 'Project Tasks')} ({tasks.length})
            </h4>
            <div className="space-y-2">
              {tasks.map((task) => (
                <div key={task.id} className="rounded-lg border p-3">
                  <div className="flex items-center justify-between gap-2">
                    <p className="text-sm font-medium">{task.name}</p>
                    <StatusBadge status={task.type || 'pending'} />
                  </div>
                  <p className="text-xs text-muted-foreground mt-1">{task.description || '-'}</p>
                  <p className="text-[11px] text-muted-foreground mt-1">
                    {t('עובד', 'Worker')}: {task.userName || '-'}
                  </p>
                </div>
              ))}
              {tasks.length === 0 && (
                <p className="text-sm text-muted-foreground">{t('אין משימות לפרויקט זה', 'No tasks for this project')}</p>
              )}
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
};

export default ProjectDetailsDialog;
