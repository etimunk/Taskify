import { create } from 'zustand';

type Lang = 'he' | 'en';

interface LangState {
  lang: Lang;
  isRtl: boolean;
  toggle: () => void;
  t: (he: string, en: string) => string;
}

export const useLanguage = create<LangState>((set, get) => ({
  lang: (localStorage.getItem('taskify_lang') as Lang) || 'he',
  isRtl: (localStorage.getItem('taskify_lang') || 'he') === 'he',
  toggle: () => {
    const newLang = get().lang === 'he' ? 'en' : 'he';
    localStorage.setItem('taskify_lang', newLang);
    set({ lang: newLang, isRtl: newLang === 'he' });
  },
  t: (he: string, en: string) => get().lang === 'he' ? he : en,
}));
