import React from 'react';
import { create } from 'zustand';

interface SourceState {
    tabKeys: string[];
    isLoading: boolean;
    error: string | null;
    fetchTabKeys: () => Promise<void>;
}

export const useAllSourceStore = create<SourceState>((set) => ({
    tabKeys: [],
    isLoading: false,
    error: null,
    fetchTabKeys: async () => {
        set({ isLoading: true, error: null });
        try {
            const response = await fetch('/api/Sniffer/GetAllSource');
            if (!response.ok) {
                throw new Error('Failed to fetch tab keys');
            }
            const data = await response.json();
            set({ tabKeys: data, isLoading: false });
        } catch (error) {
            const errorMessage = error instanceof Error ? error.message : 'An unknown error occurred';
            set({ error: errorMessage, isLoading: false });
        }
    },
}));

// 自定义 Hook，在 tabKeys 为空时自动调用 fetchTabKeys
export const useAutoFetchTabKeys = () => {
    const { tabKeys, fetchTabKeys } = useAllSourceStore();
    const isTabKeysEmpty = tabKeys.length === 0;

    // 使用 useEffect 在组件挂载时检查 tabKeys 是否为空
    // 如果为空，则调用 fetchTabKeys 方法
    React.useEffect(() => {
        if (isTabKeysEmpty) {
            fetchTabKeys();
        }
    }, [isTabKeysEmpty, fetchTabKeys]);

    return useAllSourceStore();
};