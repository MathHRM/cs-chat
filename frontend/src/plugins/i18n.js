import { createI18n } from 'vue-i18n';
import languages from '../i18n';

const i18n = createI18n({
    legacy: false,
    locale: 'en-US',
    fallbackLocale: 'en-US',
    messages: languages
});

export default i18n;