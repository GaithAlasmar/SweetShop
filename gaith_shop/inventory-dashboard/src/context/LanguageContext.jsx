import React, { createContext, useContext, useState, useEffect } from 'react';
import { translations } from '../data/translations';

const LanguageContext = createContext();

export const LanguageProvider = ({ children }) => {
    // Default to Arabic (RTL) as per current App.css, or detect/localstorage
    const [language, setLanguage] = useState(() => {
        return localStorage.getItem('app-language') || 'ar';
    });

    const direction = language === 'ar' ? 'rtl' : 'ltr';

    useEffect(() => {
        // Update HTML attribute for global direction
        document.documentElement.setAttribute('dir', direction);
        document.documentElement.setAttribute('lang', language);
        localStorage.setItem('app-language', language);
    }, [language, direction]);

    const toggleLanguage = () => {
        setLanguage(prev => prev === 'ar' ? 'en' : 'ar');
    };

    // Simple translation helper
    const t = (key) => {
        // Import here or pass as prop, but for simplicity assuming we import it
        // We need to move translations import inside or outside.
        // Let's rely on the file we just created.
        return translations[language][key] || key;
    };

    return (
        <LanguageContext.Provider value={{ language, direction, toggleLanguage, t }}>
            {children}
        </LanguageContext.Provider>
    );
};

export const useLanguage = () => {
    const context = useContext(LanguageContext);
    if (!context) {
        throw new Error('useLanguage must be used within a LanguageProvider');
    }
    return context;
};
