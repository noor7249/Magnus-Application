import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { authApi } from '../api/authApi'

export const useAuthStore = create(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      refreshTokenValue: null,
      expiresAtUtc: null,
      isAuthenticated: false,

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
        const { token, refreshTokenValue } = get()
        if (!token || !refreshTokenValue) {
          get().logout()
          return null
        }

        const session = await authApi.refreshToken({
          accessToken: token,
          refreshToken: refreshTokenValue,
        })
        get().setSession(session)
        return session
      },

      setSession: (session) => {
        set({
          token: session.accessToken,
          refreshTokenValue: session.refreshToken,
          expiresAtUtc: session.expiresAtUtc,
          user: {
            email: session.email,
            fullName: session.fullName,
            roles: session.roles || [],
          },
          isAuthenticated: true,
        })
      },

      logout: () => {
        set({
          user: null,
          token: null,
          refreshTokenValue: null,
          expiresAtUtc: null,
          isAuthenticated: false,
        })
      },

      hasAnyRole: (roles = []) => {
        const userRoles = get().user?.roles || []
        return roles.length === 0 || roles.some((role) => userRoles.includes(role))
      },
    }),
    {
      name: 'magnus-auth',
      partialize: (state) => ({
        user: state.user,
        token: state.token,
        refreshTokenValue: state.refreshTokenValue,
        expiresAtUtc: state.expiresAtUtc,
        isAuthenticated: state.isAuthenticated,
      }),
    },
  ),
)
