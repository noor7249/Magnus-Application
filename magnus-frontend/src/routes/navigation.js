import { BadgeCheck, Building2, LayoutDashboard, Users } from 'lucide-react'
import { ROLES } from '../utils/constants'

export const navigationItems = [
  { to: '/dashboard', label: 'Dashboard', icon: LayoutDashboard },
  { to: '/employees', label: 'Employees', icon: Users, roles: [ROLES.admin, ROLES.manager] },
  { to: '/departments', label: 'Departments', icon: Building2 },
  { to: '/designations', label: 'Designations', icon: BadgeCheck },
]
