import {
  LayoutDashboard,
  Users,
  FolderKanban,
  ListTodo,
  LogOut,
  Globe,
  ChevronLeft,
  User,
} from 'lucide-react';
import { NavLink } from '@/components/NavLink';
import { useLocation } from 'react-router-dom';
import { useAuthStore } from '@/store/useAuthStore';
import { useLanguage } from '@/hooks/useLanguage';
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarFooter,
  useSidebar,
} from '@/components/ui/sidebar';

const navByRole = {
  headmanager: [
    { he: 'לוח בקרה', en: 'Dashboard', icon: LayoutDashboard, path: '/dashboard' },
    { he: 'משתמשים', en: 'Users', icon: Users, path: '/users' },
    { he: 'פרויקטים', en: 'Projects', icon: FolderKanban, path: '/projects' },
    { he: 'משימות', en: 'Tasks', icon: ListTodo, path: '/tasks' },
    { he: 'פרופיל', en: 'Profile', icon: User, path: '/profile' },
  ],
  manager: [
    { he: 'לוח בקרה', en: 'Dashboard', icon: LayoutDashboard, path: '/manager' },
    { he: 'הפרויקטים שלי', en: 'My Projects', icon: FolderKanban, path: '/manager/projects' },
    { he: 'משימות', en: 'Tasks', icon: ListTodo, path: '/manager/tasks' },
    { he: 'פרופיל', en: 'Profile', icon: User, path: '/profile' },
  ],
  worker: [
    { he: 'המשימות שלי', en: 'My Tasks', icon: ListTodo, path: '/worker' },
    { he: 'פרופיל', en: 'Profile', icon: User, path: '/profile' },
  ],
};

export function AppSidebar() {
  const { state } = useSidebar();
  const collapsed = state === 'collapsed';
  const location = useLocation();
  const { role, email, logout } = useAuthStore();
  const { t, toggle, lang } = useLanguage();

  const normalizedRole = (role || 'worker').toLowerCase() as keyof typeof navByRole;
  const items = navByRole[normalizedRole] ?? navByRole.worker;

  return (
    <Sidebar collapsible="icon" side={lang === 'he' ? 'right' : 'left'}>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel className="text-xs font-semibold tracking-wider uppercase">
            {!collapsed && (
              <span className="flex items-center gap-2">
                <FolderKanban className="h-4 w-4 text-accent" />
                Taskify
              </span>
            )}
          </SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {items.map((item) => (
                <SidebarMenuItem key={item.path}>
                  <SidebarMenuButton asChild>
                    <NavLink
                      to={item.path}
                      end={item.path === '/dashboard' || item.path === '/manager' || item.path === '/worker'}
                      className="transition-spring hover:bg-sidebar-accent/50"
                      activeClassName="bg-sidebar-accent text-sidebar-accent-foreground font-medium"
                    >
                      <item.icon className="h-4 w-4 shrink-0" />
                      {!collapsed && <span>{t(item.he, item.en)}</span>}
                    </NavLink>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter className="border-t p-2 space-y-1">
        {!collapsed && (
          <div className="px-2 py-1">
            {email ? (
              <a className="text-xs text-muted-foreground truncate block hover:underline" href={`mailto:${email}`}>
                {email}
              </a>
            ) : (
              <p className="text-xs text-muted-foreground truncate">-</p>
            )}
            <p className="text-[10px] text-muted-foreground/60 capitalize">{role}</p>
          </div>
        )}
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton onClick={toggle} className="transition-spring">
              <Globe className="h-4 w-4" />
              {!collapsed && <span>{lang === 'he' ? 'English' : 'עברית'}</span>}
            </SidebarMenuButton>
          </SidebarMenuItem>
          <SidebarMenuItem>
            <SidebarMenuButton onClick={logout} className="text-destructive transition-spring hover:bg-destructive/10">
              <LogOut className="h-4 w-4" />
              {!collapsed && <span>{t('התנתקות', 'Sign Out')}</span>}
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
}
