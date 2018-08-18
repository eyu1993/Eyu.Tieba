import Vue from 'vue'
import Router from 'vue-router'

import About from '../components/About.vue'
import Ban from '../components/home/Ban.vue'
import Bind from '../components/home/Bind.vue'
import Feedback from '../components/Feedback.vue'
import FindPassword from '../components/user/FindPassword.vue'
import Index from '../components/home/Index.vue'
import Login from '../components/user/Login.vue'
import Register from '../components/user/Register.vue'
import Reply from '../components/home/Reply.vue'
import Setting from '../components/user/Setting.vue'
import Tip from '../components/Tip.vue'

Vue.use(Router)

export default new Router({
  routes: [
    {
      name: '/',
      path: '/',
      component: Index,
      beforeEnter: requireAuth
    },
    {
      name: 'about',
      path: '/about',
      component: About
    },
    {
      name: 'ban',
      path: '/home/ban',
      component: Ban,
      beforeEnter: requireAuth
    },
    {
      name: 'bind',
      path: '/home/bind',
      component: Bind,
      beforeEnter: requireAuth
    },
    {
      name: 'feedback',
      path: '/feedback',
      component: Feedback
    },
    {
      name: 'findPassword',
      path: '/user/findPassword',
      component: FindPassword
    },
    {
      name: 'index',
      path: '/home/index',
      component: Index,
      beforeEnter: requireAuth
    },
    {
      name: 'login',
      path: '/user/login',
      component: Login
    },
    {
      name: 'register',
      path: '/user/register',
      component: Register
    },
    {
      name: 'reply',
      path: '/home/reply',
      component: Reply,
      beforeEnter: requireAuth
    },
    {
      name: 'setting',
      path: '/user/setting',
      component: Setting,
      beforeEnter: requireAuth
    },
    {
      name: 'tip',
      path: '/tip',
      component: Tip
    }
  ]
})
function requireAuth (to, from, next) {
  let token = localStorage['token'];
  if (token == null) {
    next({
      path: '/user/login',
      query: {
        redirect: to.fullPath
      }
    });
  } else {
    next();
  }
}
