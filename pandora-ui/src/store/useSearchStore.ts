import { create } from 'zustand';

interface SearchState {
    searchResults: any[];
    error: string | null;
    isLoading: boolean;
    search: (query: string) => Promise<void>;
}

export const useSearchStore = create<SearchState>((set) => ({
    searchResults: [],
    error: null,
    isLoading: false,
    search: async (query: string) => {
        set({ isLoading: true, error: null });
        try {
            const response = await fetch(`/api/search/${query}`);
            if (response.ok) {
                const data = await response.json();
                set({ searchResults: data, isLoading: false });
            } else {
                const errorText = await response.text();
                set({ error: `Error: ${response.status} - ${errorText}`, isLoading: false });
            }
        } catch (error: any) {
            set({ error: `Error: ${error.message}`, isLoading: false });
        }
    },
}));
