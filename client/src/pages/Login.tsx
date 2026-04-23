import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { useAuthStore } from '@/store/useAuthStore';
import { useLanguage } from '@/hooks/useLanguage';
import { authApi } from '@/api/services';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { LogIn, Loader2, Globe, FolderKanban, Lock } from 'lucide-react';
import { toast } from 'sonner';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const { setToken } = useAuthStore();
  const { t, toggle, lang } = useLanguage();
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email.trim()) {
      toast.error(t('נא להזין אימייל', 'Please enter email'));
      return;
    }
    setLoading(true);
    try {
      const res = await authApi.login({ email: email.trim(), password: password });
      console.log('LOGIN RESPONSE:', res.status, res.data);
      setToken(res.data.token);
      const store = useAuthStore.getState();
      const role = store.role;

      if (role === 'headmanager') navigate('/dashboard');
      else if (role === 'manager') navigate('/manager');
      else navigate('/worker');

      toast.success(t('התחברת בהצלחה!', 'Logged in successfully!'));
    } catch (err: any) {
      console.log('LOGIN ERROR:', err?.response?.status, err?.response?.data);
      const msg =
        err?.response?.data?.message ||
        (typeof err?.response?.data === 'string' ? err.response.data : '') ||
        t('שגיאה בהתחברות', 'Login failed');
      toast.error(msg || t('שגיאה בהתחברות', 'Login failed'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4" dir={lang === 'he' ? 'rtl' : 'ltr'}>
      <button
        onClick={toggle}
        className="absolute top-4 left-4 p-2 rounded-lg bg-secondary text-secondary-foreground transition-spring active-scale"
      >
        <Globe className="h-4 w-4" />
        <span className="text-xs ml-1">{lang === 'he' ? 'EN' : 'עב'}</span>
      </button>

      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.4, ease: [0.4, 0, 0.2, 1] }}
        className="w-full max-w-sm"
      >
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-12 h-12 rounded-xl gradient-accent mb-4">
            <FolderKanban className="h-6 w-6 text-accent-foreground" />
          </div>
          <h1 className="text-2xl text-display text-foreground">Taskify</h1>
          <p className="text-muted-foreground text-sm mt-1">
            {t('מערכת ניהול פרויקטים ומשימות', 'Project & Task Management')}
          </p>
        </div>

        <div className="bg-card rounded-xl border shadow-card p-6">
          <form onSubmit={handleLogin} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email" className="text-sm font-medium">
                {t('אימייל', 'Email')}
              </Label>
              <Input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder={t('הזן את האימייל שלך', 'Enter your email')}
                className="h-11"
                autoFocus
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="password" className="text-sm font-medium">
                {t('סיסמה', 'Password')}
              </Label>
              <Input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder={t('הזן סיסמה', 'Enter password')}
                className="h-11"
              />
            </div>

            <Button
              type="submit"
              disabled={loading}
              className="w-full h-11 gradient-accent text-accent-foreground font-medium active-scale"
            >
              {loading ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                <>
                  <LogIn className="h-4 w-4 me-2" />
                  {t('התחברות', 'Sign In')}
                </>
              )}
            </Button>
          </form>
        </div>

        <p className="text-center text-xs text-muted-foreground mt-4">
          {t('הרשמה מתבצעת על ידי מנהל המערכת', 'Registration is managed by the system admin')}
        </p>
      </motion.div>
    </div>
  );
};

export default Login;
