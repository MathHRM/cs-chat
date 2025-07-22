import { createI18n } from 'vue-i18n';
import languages from '../i18n';

const i18n = createI18n({
    legacy: false,
    locale: 'pt-BR',
    fallbackLocale: 'pt-BR',
    messages: languages
});

export default i18n;