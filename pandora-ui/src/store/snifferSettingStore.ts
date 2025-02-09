import { create } from 'zustand';

interface FormItemsState {
  snifferSettings: Record<string, Record<string, string>>;
  isLoading: Record<string, boolean>;
  error: Record<string, string | null>;
  fetchFormItems: (source: string) => Promise<void>
}

export const useSnifferSettingStore = create<FormItemsState>((set) => ({
  snifferSettings: {},
  isLoading: {},
  error: {},
  fetchFormItems: async (source: string) => {
    set((state) => ({
      isLoading: { ...state.isLoading, [source]: true },
      error: { ...state.error, [source]: null }
    }));
    try {
      let response = await fetch(`api/sniffer/GetAllConfiguration/${source}`);
      if (!response.ok) {
        throw new Error('Failed to fetch form items');
      }
      const config: Record<string, string> = await response.json();
      response = await fetch(`api/sniffer/GetAllKeys/${source}`);
      if (!response.ok) {
        throw new Error('Failed to fetch form items');
      }
      const keys = await response.json();
      for (const key of keys) {
        if (!config[key]) {
          config[key] = '';
        }
      }
      set((state) => ({
        snifferSettings: { ...state.snifferSettings, [source]: config }
      }));


    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'An unknown error occurred';
      set((state) => ({
        error: { ...state.error, [source]: errorMessage }
      }));
    } finally {
      set((state) => ({
        isLoading: { ...state.isLoading, [source]: false }
      }));
    }
  }
}));
