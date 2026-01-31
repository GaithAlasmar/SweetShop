import React from 'react';
import { FaFilter } from 'react-icons/fa';

const FilterPanel = ({ filters, onFilterChange }) => {
    const categories = ['الكل', 'إلكترونيات', 'ملحقات', 'إكسسوارات'];
    const statuses = ['الكل', 'متوفر', 'منخفض'];
    const priceRanges = [
        { label: 'الكل', value: 'all' },
        { label: 'أقل من 200', value: '0-200' },
        { label: '200 - 500', value: '200-500' },
        { label: '500 - 1000', value: '500-1000' },
        { label: 'أكثر من 1000', value: '1000+' }
    ];

    return (
        <div className="filter-panel">
            <div className="filter-header">
                <FaFilter />
                <h3>الفلاتر</h3>
            </div>

            <div className="filter-group">
                <label>الفئة</label>
                <select
                    value={filters.category}
                    onChange={(e) => onFilterChange('category', e.target.value)}
                >
                    {categories.map(cat => (
                        <option key={cat} value={cat}>{cat}</option>
                    ))}
                </select>
            </div>

            <div className="filter-group">
                <label>الحالة</label>
                <select
                    value={filters.status}
                    onChange={(e) => onFilterChange('status', e.target.value)}
                >
                    {statuses.map(status => (
                        <option key={status} value={status}>{status}</option>
                    ))}
                </select>
            </div>

            <div className="filter-group">
                <label>نطاق السعر</label>
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
                إعادة تعيين الفلاتر
            </button>
        </div>
    );
};

export default FilterPanel;
