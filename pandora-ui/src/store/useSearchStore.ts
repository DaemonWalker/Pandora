import { create } from 'zustand';
import { SearchResultModel } from '../models/SearchResultModel';
import { ALL } from '../models/Constants';

interface SearchState {
    searchResults: SearchResultModel[];
    error: string | null;
    isLoading: boolean;
    search: (source: string, query: string) => Promise<void>;
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
