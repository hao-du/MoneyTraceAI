import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

const resources = {
    en: {
        translation: {
            "welcome": "Welcome to MoneyTrace",
            "login": "Login",
            "dashboard": "Dashboard",
            "transactions": "Transactions",
            "settings": "Settings",
            "logout": "Logout",
        }
    },
    vi: {
        translation: {
            "welcome": "Chào mừng đến với MoneyTrace",
            "login": "Đăng nhập",
            "dashboard": "Bảng điều khiển",
            "transactions": "Giao dịch",
            "settings": "Cài đặt",
            "logout": "Đăng xuất",
        }
    }
};

i18n
    .use(initReactI18next)
    .init({
        resources,
        lng: "en", // default language
        fallbackLng: "en",
        interpolation: {
            escapeValue: false
        }
    });

export default i18n;
