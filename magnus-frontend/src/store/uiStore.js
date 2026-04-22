import { create } from 'zustand'

export const useUiStore = create((set) => ({
  loading: false,
  globalError: null,
  toast: null,

  setLoading: (loading) => set({ loading }),
  setGlobalError: (globalError) => set({ globalError }),
  clearGlobalError: () => set({ globalError: null }),
  showToast: (message, type = 'success') => set({ toast: { message, type, id: Date.now() } }),
  clearToast: () => set({ toast: null }),
}))
