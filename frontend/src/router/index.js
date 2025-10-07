import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '../views/LoginView.vue';
import HomeView from '../views/HomeView.vue';
import SignUpView from '../views/SignUpView.vue';
import CreateTourView from '../views/CreateTourView.vue';
import ToursView from '../views/ToursView.vue';
import ProfileView from '../views/ProfileView.vue';
import BlogView from '../views/BlogView.vue';
import CreateBlogView from '../views/CreateBlogView.vue';
import UsersView from '../views/UsersView.vue';
import { store } from '@/utils/store';

const routes = [
  { path: '/', name: 'Home', component: HomeView },
  { path: '/login', name: 'Login', component: LoginView },
  { path: '/signup', name: 'SignUp', component: SignUpView },
  { 
    path: '/create-tour', 
    name: 'CreateTour', 
    component: CreateTourView,
    meta: { requiresAuth: true, role: ['Author'] }
  },
  { 
    path: '/tours', 
    name: 'Tours', 
    component: ToursView,
    meta: { requiresAuth: true, role: ['Author'] }
  },
  { 
    path: '/profile/:username', 
    name: 'Profile', 
    component: ProfileView,
    meta: { requiresAuth: true }
  },
  { 
    path: '/blogs', 
    name: 'Blogs', 
    component: BlogView,
    meta: { requiresAuth: true }
  },
  { 
    path: '/create-blog', 
    name: 'CreateBlog', 
    component: CreateBlogView,
    meta: { requiresAuth: true, role: ['Author'] }
  },
  { 
    path: '/users', 
    name: 'Users', 
    component: UsersView,
    meta: { requiresAuth: true }
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  const userRole = store.role;
  const isLoggedIn = store.role !== 'guest';

  console.log('Router guard:', { to: to.path, userRole, isLoggedIn, requiredRole: to.meta.role });

  if (to.meta.requiresAuth && !isLoggedIn) {
    console.log('Not logged in, redirecting to /login');
    return next('/login');
  }

  if (to.meta.role && !to.meta.role.includes(userRole)) {
    console.log('Role check failed. User role:', userRole, 'Required:', to.meta.role);
    return next('/');
  }

  if (isLoggedIn && (to.path === '/login' || to.path === '/signup')) {
    console.log('Already logged in, redirecting to /');
    return next('/');
  }

  console.log('Navigation allowed to:', to.path);
  next();
});

export default router;
