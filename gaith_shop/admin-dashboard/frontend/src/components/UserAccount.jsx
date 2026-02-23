import React from 'react';
import { User } from 'lucide-react';

/**
 * دالة لاستخراج الحروف الأولى من الاسم
 * @param {string} name - اسم المستخدم الكامل
 * @returns {string} - الحروف الأولى (Initials)
 */
export const getInitials = (name) => {
    if (!name || typeof name !== 'string') return '';

    const trimmedName = name.trim();
    const nameParts = trimmedName.split(/\s+/);

    if (nameParts.length >= 2) {
        // إذا كان الاسم مكوناً من كلمتين أو أكثر، نأخذ أول حرف من الاسم الأول وأول حرف من الأخير
        const firstInitial = nameParts[0].charAt(0);
        const lastInitial = nameParts[nameParts.length - 1].charAt(0);
        return `${firstInitial} ${lastInitial}`.toUpperCase();
    } else if (nameParts.length === 1 && nameParts[0].length > 0) {
        // إذا كان كلمة واحدة، نأخذ أول حرفين
        return nameParts[0].slice(0, 2).toUpperCase();
    }

    return '';
};

const UserAccount = ({ isLoggedIn, user }) => {
    // استخراج الحروف الأولى في حال تسجيل الدخول
    const initials = isLoggedIn && user?.name ? getInitials(user.name) : '';

    return (
        <div className="flex items-center gap-3">
            {isLoggedIn ? (
                // حالة تسجيل الدخول: عرض دائرة الحروف (Avatar)
                <div
                    className="flex items-center justify-center w-10 h-10 rounded-full bg-primary text-white font-bold text-sm shadow-md transition-transform hover:scale-105"
                    aria-label={`User avatar for ${user.name}`}
                >
                    {initials}
                </div>
            ) : (
                // حالة عدم تسجيل الدخول: عرض الأيقونة الافتراضية مع النص
                <button
                    className="flex items-center gap-2 px-3 py-2 rounded-lg text-gray-700 hover:bg-gray-100 transition-colors group"
                >
                    <span className="font-medium text-sm">حسابي</span>
                    <div className="p-1.5 rounded-full bg-gray-50 group-hover:bg-white border border-transparent group-hover:border-gray-200 transition-all">
                        <User size={20} className="text-gray-500 group-hover:text-primary" />
                    </div>
                </button>
            )}
        </div>
    );
};

export default UserAccount;
