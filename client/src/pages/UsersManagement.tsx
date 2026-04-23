import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { Users, Plus, Pencil, Trash2, Shield, X } from 'lucide-react';
import { usersApi } from '@/api/services';
import { useLanguage } from '@/hooks/useLanguage';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger } from '@/components/ui/dialog';
import { toast } from 'sonner';
import type { UserDTO } from '@/types';

const levelToNumber = (level: string) => {
  if (level === 'manager') return 1;
  if (level === 'headmanager') return 2;
  return 0;
};

const normalizeLevel = (level: string | number) => {
  if (typeof level === 'number') {
    if (level === 1) return 'manager';
    if (level === 2) return 'headmanager';
    return 'worker';
  }
  return level || 'worker';
};

const UsersManagement = () => {
  const { t, lang } = useLanguage();
  const qc = useQueryClient();
  const isHebrew = lang === 'he';
  const [open, setOpen] = useState(false);
  const [editUser, setEditUser] = useState<UserDTO | null>(null);
  const [roleFilter, setRoleFilter] = useState<'all' | 'worker' | 'manager' | 'headmanager'>('all');
  const [form, setForm] = useState({ tz: '', name: '', email: '', password: '', level: 'worker' });

  const { data: users = [] } = useQuery({ queryKey: ['users'], queryFn: () => usersApi.getAll().then(r => r.data) });

  const createMutation = useMutation({
    mutationFn: (data: any) => usersApi.create(data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['users'] }); setOpen(false); resetForm(); toast.success(t('משתמש נוסף', 'User added')); },
    onError: () => toast.error(t('שגיאה', 'Error')),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: any }) => usersApi.update(id, data),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['users'] }); setEditUser(null); toast.success(t('עודכן', 'Updated')); },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: number) => usersApi.delete(id),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['users'] }); toast.success(t('נמחק', 'Deleted')); },
  });

  const resetForm = () => setForm({ tz: '', name: '', email: '', password: '', level: 'worker' });

  const handleCreate = (e: React.FormEvent) => {
    e.preventDefault();
    createMutation.mutate({
      id: 0,
      tz: form.tz,
      name: form.name,
      email: form.email,
      password: form.password,
      role: form.level,
      level: levelToNumber(form.level),
    });
  };

  const handleRoleChange = (user: UserDTO, newLevel: string) => {
    updateMutation.mutate({
      id: user.id,
      data: {
        ...user,
        role: newLevel,
        level: levelToNumber(newLevel),
      },
    });
  };

  const roleLabel = (level: string) => {
    if (level === 'headmanager') return t('מנהל ראשי', 'Head Manager');
    if (level === 'manager') return t('מנהל', 'Manager');
    return t('עובד', 'Worker');
  };

  const roleBadgeClass = (level: string) => {
    if (level === 'headmanager') return 'bg-accent/10 text-accent';
    if (level === 'manager') return 'bg-info/10 text-info';
    return 'bg-muted text-muted-foreground';
  };

  const filteredUsers = users.filter((user) => {
    if (roleFilter === 'all') return true;
    return normalizeLevel(user.level as any) === roleFilter;
  });

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl text-display text-foreground">{t('ניהול משתמשים', 'Users Management')}</h1>
          <p className="text-muted-foreground text-sm">{filteredUsers.length} {t('משתמשים', 'users')}</p>
        </div>
        <div className="flex items-center gap-2">
          <Select value={roleFilter} onValueChange={(val: 'all' | 'worker' | 'manager' | 'headmanager') => setRoleFilter(val)}>
            <SelectTrigger className="w-44">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">{t('כל התפקידים', 'All roles')}</SelectItem>
              <SelectItem value="worker">{t('עובד', 'Worker')}</SelectItem>
              <SelectItem value="manager">{t('מנהל', 'Manager')}</SelectItem>
              <SelectItem value="headmanager">{t('מנהל ראשי', 'Head Manager')}</SelectItem>
            </SelectContent>
          </Select>

          <Dialog open={open} onOpenChange={setOpen}>
            <DialogTrigger asChild>
              <Button className="gradient-accent text-accent-foreground active-scale">
                <Plus className="h-4 w-4 me-1" /> {t('הוסף משתמש', 'Add User')}
              </Button>
            </DialogTrigger>
            <DialogContent dir={isHebrew ? 'rtl' : 'ltr'} className={isHebrew ? 'text-right' : ''}>
              <DialogHeader>
                <DialogTitle>{t('משתמש חדש', 'New User')}</DialogTitle>
              </DialogHeader>
              <form onSubmit={handleCreate} className="space-y-3">
                <div><Label>{t('ת.ז', 'ID')}</Label><Input value={form.tz} onChange={e => setForm({ ...form, tz: e.target.value })} required /></div>
                <div><Label>{t('שם', 'Name')}</Label><Input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required /></div>
                <div><Label>{t('אימייל', 'Email')}</Label><Input type="email" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} required /></div>
                <div><Label>{t('סיסמה', 'Password')}</Label><Input type="password" value={form.password} onChange={e => setForm({ ...form, password: e.target.value })} required /></div>
                <div>
                  <Label>{t('רמה', 'Level')}</Label>
                  <Select value={form.level} onValueChange={(val) => setForm({ ...form, level: val })}>
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="worker">{t('עובד', 'Worker')}</SelectItem>
                      <SelectItem value="manager">{t('מנהל', 'Manager')}</SelectItem>
                      <SelectItem value="headmanager">{t('מנהל ראשי', 'Head Manager')}</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <Button type="submit" className="w-full gradient-accent text-accent-foreground active-scale">
                  {t('הוסף', 'Add')}
                </Button>
              </form>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      <div className="bg-card rounded-xl border shadow-card overflow-hidden">
        <div className="divide-y">
          {filteredUsers.map((user, i) => (
            <motion.div
              key={user.id}
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ delay: i * 0.03 }}
              className="px-5 py-3.5 flex items-center justify-between gap-4 transition-spring hover:bg-muted/30"
            >
              <div className="flex items-center gap-3 min-w-0">
                <div className="w-8 h-8 rounded-full gradient-accent flex items-center justify-center text-accent-foreground text-xs font-bold shrink-0">
                  {user.name?.[0]?.toUpperCase()}
                </div>
                <div className="min-w-0">
                  <p className="text-sm font-medium text-foreground truncate">{user.name}</p>
                  <a className="text-xs text-muted-foreground truncate block hover:underline" href={`mailto:${user.email}`}>
                    {user.email}
                  </a>
                </div>
              </div>

              <div className="flex items-center gap-2 shrink-0">
                <Select value={normalizeLevel(user.level as any)} onValueChange={(val) => handleRoleChange(user, val)}>
                  <SelectTrigger className="h-7 text-xs w-32">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="worker">{t('עובד', 'Worker')}</SelectItem>
                    <SelectItem value="manager">{t('מנהל', 'Manager')}</SelectItem>
                    <SelectItem value="headmanager">{t('מנהל ראשי', 'Head Manager')}</SelectItem>
                  </SelectContent>
                </Select>

                <Button
                  variant="ghost"
                  size="icon"
                  className="h-7 w-7 text-destructive hover:bg-destructive/10"
                  onClick={() => deleteMutation.mutate(user.id)}
                >
                  <Trash2 className="h-3.5 w-3.5" />
                </Button>
              </div>
            </motion.div>
          ))}
          {filteredUsers.length === 0 && (
            <div className="p-12 text-center text-sm text-muted-foreground">{t('אין משתמשים', 'No users')}</div>
          )}
        </div>
      </div>
    </div>
  );
};

export default UsersManagement;
