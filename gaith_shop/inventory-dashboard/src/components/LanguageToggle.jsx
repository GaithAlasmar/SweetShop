import React from 'react';
import { FaGlobe } from 'react-icons/fa';
import { useLanguage } from '../context/LanguageContext';

const LanguageToggle = () => {
    const { language, toggleLanguage } = useLanguage();

    return (
        <button
            onClick={toggleLanguage}
            className="language-toggle"
            aria-label="Toggle Language"
        >
            <FaGlobe />
            <span>{language === 'ar' ? 'English' : 'العربية'}</span>
        </button>
    );
};

export default LanguageToggle;
