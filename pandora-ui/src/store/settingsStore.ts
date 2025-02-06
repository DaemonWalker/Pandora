import { create } from 'zustand';

interface SettingsState {
    tabKeys: string[];
    isLoading: boolean;
    error: string | null;
    fetchTabKeys: () => Promise<void>;
}

export const useSettingsStore = create<SettingsState>((set) => ({
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

