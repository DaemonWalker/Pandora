import { create } from 'zustand';
import { SearchResultModel } from '../models/SearchResultModel';
import { ALL } from '../models/Constants';

interface SearchState {
    searchResults: SearchResultModel[];
    error: string | null;
    isLoading: boolean;
    search: (source: string, query: string) => Promise<void>;
    clearError: () => void
}

export const useSearchStore = create<SearchState>((set) => ({
    searchResults: [],
    error: null,
    isLoading: false,
    search: async (source: string, query: string) => {
        set({ isLoading: true, error: null });
        try {
            const url = source === ALL ? `/api/Search/Search/${query}` : `/api/Search/SearchBySource/${source}/${query}`;
            const response = await fetch(url);
            if (response.ok) {
                const data = await response.json();
                set({ searchResults: data, error: null, isLoading: false });
            } else {
                const errorText = await response.text();
                console.error("catch", errorText)
                set({ searchResults: [], error: `${errorText}`, isLoading: false });
                console.error("catch", errorText)
            }
        } catch (error: any) {
            console.error(error)
            set({ error: `${error.message}`, isLoading: false });
        }
    },
    clearError: () => {
        set({ error: null });
    }
}));
