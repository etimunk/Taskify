import { useLanguage } from '@/hooks/useLanguage';

interface StatusBadgeProps {
  status: string;
}

const statusConfig: Record<string, { he: string; en: string; className: string }> = {
  pending: { he: 'ממתין', en: 'Pending', className: 'bg-warning/10 text-warning border-warning/20' },
  'in-progress': { he: 'בביצוע', en: 'In Progress', className: 'bg-info/10 text-info border-info/20' },
  done: { he: 'הושלם', en: 'Done', className: 'bg-success/10 text-success border-success/20' },
  active: { he: 'פעיל', en: 'Active', className: 'bg-success/10 text-success border-success/20' },
  completed: { he: 'הושלם', en: 'Completed', className: 'bg-muted text-muted-foreground border-muted' },
  overdue: { he: 'באיחור', en: 'Overdue', className: 'bg-destructive/10 text-destructive border-destructive/20' },
};

const StatusBadge = ({ status }: StatusBadgeProps) => {
  const { t } = useLanguage();
  const config = statusConfig[status?.toLowerCase()] || statusConfig.pending;

  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${config.className}`}>
      {t(config.he, config.en)}
    </span>
  );
};

export default StatusBadge;
