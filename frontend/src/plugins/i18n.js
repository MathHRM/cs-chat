import { createI18n } from 'vue-i18n';
import languages from '../locales';

const i18n = createI18n({
  locale: 'en-US',
  fallbackLocale: 'en-US',
  messages: languages,
});

export default i18n;