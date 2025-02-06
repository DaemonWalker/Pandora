import { create } from 'zustand';

interface FormItemsState {
  snifferSettings: Record<string, string[]>;
  isLoading: Record<string, boolean>;
  error: Record<string, string | null>;
  fetchFormItems: (source: string) => Promise<void>;
}

export const useSnifferSettingStore = create<FormItemsState>((set) => ({
  snifferSettings: {},
  isLoading: {},
  error: {},
  fetchFormItems: async (source: string) => {
    set((state) => ({
      isLoadingMap: { ...state.isLoading, [source]: true },
      error: { ...state.error, [source]: null }
    }));
    try {
      const response = await fetch(`api/sniffer/GetAllKeys/${source}`);
      if (!response.ok) {
        throw new Error('Failed to fetch form items');
      }
      const data = await response.json();
      set((state) => ({
        snifferSettings: { ...state.snifferSettings, [source]: data },
        isLoadingMap: { ...state.isLoading, [source]: false }
      }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'An unknown error occurred';
      set((state) => ({
        error: { ...state.error, [source]: errorMessage },
        isLoadingMap: { ...state.isLoading, [source]: false }
      }));
    }
  }
}));
