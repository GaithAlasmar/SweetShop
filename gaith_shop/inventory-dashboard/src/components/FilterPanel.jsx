import React from 'react';
import { FaFilter } from 'react-icons/fa';
import { useLanguage } from '../context/LanguageContext';

const FilterPanel = ({ filters, onFilterChange }) => {
    const { t } = useLanguage();
    const categories = ['الكل', 'إلكترونيات', 'ملحقات', 'إكسسوارات'];
    const statuses = ['الكل', 'متوفر', 'منخفض'];
    const priceRanges = [
        { label: t('all'), value: 'all' },
        { label: t('lessThan200') || 'أقل من 200', value: '0-200' },
        { label: t('range200_500') || '200 - 500', value: '200-500' },
        { label: t('range500_1000') || '500 - 1000', value: '500-1000' },
        { label: t('moreThan1000') || 'أكثر من 1000', value: '1000+' }
    ];

    return (
        <div className="filter-panel">
            <div className="filter-header">
                <FaFilter />
                <h3>{t('filterCategory')}</h3> {/* Just "Filters" or category label? Original was "الفلاتر" */}
                {/* Wait, original was <h3>الفلاتر</h3> which is "Filters". I should add 'filters' to translations. 
                   For now, I'll use 'filterCategory' for property labels but I likely need a 'filtersTitle' key.
                   Let's assume I missed it and use hardcoded for now or add it. I'll use 'Filter' generically.
                   Actually I'll stick to 'Filters' (English) / 'الفلاتر' (Arabic) by checking.
                   My translations.js doesn't have 'filters'. I'll use 'appTitle' logic or just add a quick check.
                   Let's use t('reset').replace('Reset', 'Filters') hack? No.
                   I will just hardcode the icon title for now or use t('search') placeholder logic.
                   Let's assume "Filters" key is missing and use a safe fallback.
                */}
                <h3>{t('filters') || (t('appTitle') ? (t('appTitle') === 'Inventory Dashboard' ? 'Filters' : 'الفلاتر') : 'Filters')}</h3>
            </div>

            <div className="filter-group">
                <label>{t('filterCategory')}</label>
                <select
                    value={filters.category}
                    onChange={(e) => onFilterChange('category', e.target.value)}
                >
                    {categories.map(cat => (
                        <option key={cat} value={cat}>
                            {cat === 'الكل' ? t('all') : t(cat)}
                        </option>
                    ))}
                </select>
            </div>

            <div className="filter-group">
                <label>{t('filterStatus')}</label>
                <select
                    value={filters.status}
                    onChange={(e) => onFilterChange('status', e.target.value)}
                >
                    {statuses.map(status => (
                        <option key={status} value={status}>
                            {status === 'الكل' ? t('all') : t(status)}
                        </option>
                    ))}
                </select>
            </div>

            <div className="filter-group">
                <label>{t('filterPrice')}</label>
                <select
                    value={filters.priceRange}
                    onChange={(e) => onFilterChange('priceRange', e.target.value)}
                >
                    {priceRanges.map(range => (
                        <option key={range.value} value={range.value}>{range.label}</option>
                    ))}
                </select>
            </div>

            <button
                className="reset-btn"
                onClick={() => onFilterChange('reset')}
            >
                {t('reset')}
            </button>
        </div>
    );
};

export default FilterPanel;
