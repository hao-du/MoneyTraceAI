import { describe, it, expect } from 'vitest';

// Utility: format a number as currency string
export function formatAmount(amount: number, symbol = ''): string {
    return `${symbol}${amount.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}`;
}

// Utility: check if a string is a valid positive number
export function isPositiveNumber(value: string): boolean {
    const num = parseFloat(value);
    return !isNaN(num) && num > 0;
}

// Utility: split comma-separated tag strings into trimmed arrays
export function parseTags(tags: string | undefined): string[] {
    if (!tags) return [];
    return tags.split(',').map((t) => t.trim()).filter(Boolean);
}

// Utility: determine if a transaction is effectively income based on type
export function isTransactionIncome(type: string, isIncome: boolean): boolean {
    if (type === 'Cashflow') return isIncome;
    if (type === 'Transfer') return isIncome; // Borrow/Collect are income, mapped externally
    if (type === 'Exchange') return isIncome;
    return false; // Bank transactions are outflows by default
}

// ============ Tests ============

describe('formatAmount', () => {
    it('formats a whole number with 2 decimal places', () => {
        expect(formatAmount(1000)).toBe('1,000.00');
    });

    it('includes currency symbol when provided', () => {
        expect(formatAmount(500, '$')).toBe('$500.00');
    });

    it('handles zero', () => {
        expect(formatAmount(0)).toBe('0.00');
    });

    it('handles decimal values', () => {
        expect(formatAmount(1234.5)).toBe('1,234.50');
    });
});

describe('isPositiveNumber', () => {
    it('returns true for a positive integer string', () => {
        expect(isPositiveNumber('100')).toBe(true);
    });

    it('returns true for a positive decimal string', () => {
        expect(isPositiveNumber('0.01')).toBe(true);
    });

    it('returns false for zero', () => {
        expect(isPositiveNumber('0')).toBe(false);
    });

    it('returns false for a negative value', () => {
        expect(isPositiveNumber('-5')).toBe(false);
    });

    it('returns false for non-numeric input', () => {
        expect(isPositiveNumber('abc')).toBe(false);
    });
});

describe('parseTags', () => {
    it('splits a comma-separated string into trimmed tags', () => {
        expect(parseTags('food, salary, vacation')).toEqual(['food', 'salary', 'vacation']);
    });

    it('returns empty array for undefined', () => {
        expect(parseTags(undefined)).toEqual([]);
    });

    it('returns empty array for empty string', () => {
        expect(parseTags('')).toEqual([]);
    });

    it('strips extra whitespace', () => {
        expect(parseTags('  a  ,  b  ')).toEqual(['a', 'b']);
    });
});

describe('isTransactionIncome', () => {
    it('returns true for income Cashflow', () => {
        expect(isTransactionIncome('Cashflow', true)).toBe(true);
    });

    it('returns false for expense Cashflow', () => {
        expect(isTransactionIncome('Cashflow', false)).toBe(false);
    });

    it('returns false for Bank transactions regardless of isIncome', () => {
        expect(isTransactionIncome('Bank', true)).toBe(false);
        expect(isTransactionIncome('Bank', false)).toBe(false);
    });
});
