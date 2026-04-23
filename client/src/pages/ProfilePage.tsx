import { useEffect, useState } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import { User } from 'lucide-react';
import { usersApi } from '@/api/services';
import { useAuthStore } from '@/store/useAuthStore';
import { useLanguage } from '@/hooks/useLanguage';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Button } from '@/components/ui/button';
import { toast } from 'sonner';

const levelToNumber = (level: string | number) => {
  if (typeof level === 'number') return level;
  if (level === 'manager') return 1;
  if (level === 'headmanager') return 2;
  return 0;
};

const normalizeRole = (role: unknown, level: string | number) => {
  if (typeof role === 'string' && role.trim()) return role;
  const lvl = levelToNumber(level);
  if (lvl === 2) return 'headmanager';
  if (lvl === 1) return 'manager';
  return 'worker';
};

const ProfilePage = () => {
  const { t } = useLanguage();
  const { userId } = useAuthStore();
  const [form, setForm] = useState({
    tz: '',
    name: '',
    email: '',
    password: '',
    role: '',
    level: 'worker',
  });

  const { data: me } = useQuery({
    queryKey: ['my-profile', userId],
    queryFn: () => usersApi.getById(userId!).then((r) => r.data),
    enabled: !!userId,
  });

  useEffect(() => {
    if (!me) return;
    setForm({
      tz: me.tz || '',
      name: me.name || '',
      email: me.email || '',
      password: '',
      role: normalizeRole(me.role, me.level),
      level: (me.level ?? 'worker') as any,
    });
  }, [me]);

  const updateMutation = useMutation({
    mutationFn: () => {
      const normalizedLevel = levelToNumber(form.level);
      const normalizedRole = normalizeRole(form.role, form.level);
      const existingPassword = ((me as any)?.password as string | undefined) || '';
      return usersApi.update(userId!, {
        id: userId!,
        tz: form.tz,
        name: form.name,
        email: form.email,
        role: normalizedRole,
        level: normalizedLevel as any,
        // API model requires Password in PUT payload.
        password: form.password.trim() || existingPassword || '',
      });
    },
    onSuccess: () => toast.success(t('הפרטים עודכנו', 'Profile updated')),
    onError: () => toast.error(t('שגיאה בעדכון הפרטים', 'Failed to update profile')),
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateMutation.mutate();
  };

  return (
    <div className="max-w-2xl">
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <User className="h-5 w-5" />
            {t('פרטים אישיים', 'Personal details')}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <form className="space-y-4" onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <Label>{t('ת.ז', 'ID')}</Label>
                <Input value={form.tz} onChange={(e) => setForm({ ...form, tz: e.target.value })} required />
              </div>
              <div>
                <Label>{t('שם', 'Name')}</Label>
                <Input value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
              </div>
            </div>
            <div>
              <Label>{t('אימייל', 'Email')}</Label>
              <Input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
            </div>
            <div>
              <Label>{t('סיסמה חדשה (לא חובה)', 'New password (optional)')}</Label>
              <Input type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} />
            </div>
            <Button type="submit" className="gradient-accent text-accent-foreground">
              {t('שמור שינויים', 'Save changes')}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
};

export default ProfilePage;
