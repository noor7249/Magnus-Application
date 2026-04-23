import { create } from 'zustand'
import { authApi } from '../api/authApi'

export const useAuthStore = create((set, get) => ({
      user: null,
      token: null,
      expiresAtUtc: null,
      isAuthenticated: false,
      isRestoringSession: true,

      login: async (credentials) => {
        const session = await authApi.login(credentials)
        get().setSession(session)
        return session
      },

      register: async (payload) => {
        const session = await authApi.register(payload)
        get().setSession(session)
        return session
      },

      refreshToken: async () => {
        const session = await authApi.refreshToken()
        get().setSession(session)
        return session
      },

      setSession: (session) => {
        set({
          token: session.accessToken,
          expiresAtUtc: session.expiresAtUtc,
          user: {
            email: session.email,
            fullName: session.fullName,
            roles: session.roles || [],
          },
          isAuthenticated: true,
          isRestoringSession: false,
        })
      },

      restoreSession: async () => {
        if (get().isAuthenticated) {
          set({ isRestoringSession: false })
          return get().isAuthenticated
        }

        set({ isRestoringSession: true })

        try {
          const session = await authApi.refreshToken()
          get().setSession(session)
          return true
        } catch {
          get().clearSession()
          return false
        } finally {
          set({ isRestoringSession: false })
        }
      },

      clearSession: () => {
        localStorage.removeItem('accessToken')
        set({
          user: null,
          token: null,
          expiresAtUtc: null,
          isAuthenticated: false,
          isRestoringSession: false,
        })
      },

      logout: async () => {
        try {
          await authApi.logout()
        } finally {
          get().clearSession()
        }
      },

      hasAnyRole: (roles = []) => {
        const userRoles = get().user?.roles || []
        return roles.length === 0 || roles.some((role) => userRoles.includes(role))
      },
}))
