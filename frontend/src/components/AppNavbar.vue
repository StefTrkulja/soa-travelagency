<template>
	<v-app-bar :elevation="1" color="primary">
		<v-btn class="navbar-title" text @click="$router.push('/')">
			<v-img src="@/assets/logo.png" alt="Travel Agency Logo" width="32" height="32" class="navbar-logo"></v-img>
			Travel Agency
		</v-btn>
		<v-spacer></v-spacer>
		<v-btn icon="mdi-menu" @click="drawer = !drawer" v-if="store.role !== 'guest'">
		</v-btn>
		<v-btn size="large" prepend-icon="mdi-login" v-if="store.role === 'guest'" to="/login">Login</v-btn>
	</v-app-bar>

	<v-navigation-drawer v-model="drawer" temporary location="right" v-if="store.role !== 'guest'">
		<v-list>
			<v-list-item size="large" prepend-icon="mdi-map" to="/tours">
				Tours
			</v-list-item>

			<v-list-item size="large" prepend-icon="mdi-plus" to="/create-tour" v-if="store.role === 'Author'">
				Create Tour
			</v-list-item>

			<v-list-item size="large" prepend-icon="mdi-blog" to="/blogs">
				Blogs
			</v-list-item>

			<v-list-item size="large" prepend-icon="mdi-plus" to="/create-blog" v-if="store.role === 'Author'">
				Create Blog
			</v-list-item>

			<v-list-item size="large" prepend-icon="mdi-account" @click="showUserProfile">
				Profile
			</v-list-item>

			<v-list-item size="large" prepend-icon="mdi-account-group" to="/users">
				Discover Users
			</v-list-item>

			<v-list-item size="large" prepend-icon="mdi-cog" to="/admin/users" v-if="store.role === 'Administrator'">
				Manage Users
			</v-list-item>
		</v-list>

		<template v-slot:append>
			<v-btn block prepend-icon="mdi-logout" v-if="store.role !== 'guest'" to="/" @click="logout">
				Logout
			</v-btn>
		</template>
	</v-navigation-drawer>
</template>

<script>
import { store } from '@/utils/store';
import axiosInstance from '@/utils/axiosInstance';
import { nextTick } from 'vue';

export default {
	computed: {
		store() {
			return store;
		}
	},
	data() {
		return {
			drawer: false,
		}
	},
	methods: {
		logout() {
			store.clearUser();
			this.$router.push('/login');
		},
		showUserProfile() {
			this.$router.push(`/profile/${store.username}`);
		},
	},
};

</script>

<style scoped>
.navbar-title {
	font-size: 1.7rem;
	font-weight: bold;
	color: #FFFFFF;
	text-transform: none;
	letter-spacing: 1px;
	transition: background-color 0.3s, color 0.3s;
	padding: 0 16px;
	display: flex;
	align-items: center;
	gap: 8px;
}

.navbar-logo {
	margin-right: 4px;
}

.navbar-title:hover {
	background-color: rgba(255, 255, 255, 0.1);
	border-radius: 8px;
	color: #BBDEFB;
	cursor: pointer;
}
</style>