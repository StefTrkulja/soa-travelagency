<template>
	<v-app-bar :elevation="1" color="primary">
		<v-btn class="navbar-title" text @click="$router.push('/')">
			<v-img src="@/assets/logo.png" alt="Travel Agency Logo" width="32" height="32" class="navbar-logo"></v-img>
			Travel Agency
		</v-btn>
		<v-spacer></v-spacer>
		<v-btn 
			size="large" 
			prepend-icon="mdi-logout" 
			variant="outlined" 
			color="white"
			class="mr-3 logout-btn"
			v-if="store.role !== 'guest'" 
			@click="logout"
		>
			Logout
		</v-btn>
		<v-btn icon="mdi-menu" @click="drawer = !drawer" v-if="store.role !== 'guest'">
		</v-btn>
		<v-btn size="large" prepend-icon="mdi-login" v-if="store.role === 'guest'" to="/login">Login</v-btn>
	</v-app-bar>

	<v-navigation-drawer v-model="drawer" temporary location="right" v-if="store.role === 'Tourist'">
		<v-list>
		
		<v-list-item 
			size="large" 
			prepend-icon="mdi-map-marker" 
			to="/my-location" 
			v-if="store.role === 'Tourist'"
			class="golden-menu-item"
		>
			My Location
		</v-list-item>

		<v-list-item size="large" prepend-icon="mdi-map" to="/tours" v-if="store.role === 'Author'">
			My Tours
		</v-list-item>

		<v-list-item size="large" prepend-icon="mdi-map-marker-multiple" to="/tours/public" v-if="store.role === 'Tourist'">
			Available Tours
		</v-list-item>

		<v-list-item size="large" prepend-icon="mdi-star-box" to="/my-reviews" v-if="store.role === 'Tourist'">
			My Reviews
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

.v-btn.logout-btn {
	border: 1px solid rgba(255, 255, 255, 0.7);
	color: white;
	transition: all 0.3s ease;
}

.v-btn.logout-btn:hover {
	background-color: rgba(255, 255, 255, 0.1);
	border-color: white;
}

.golden-menu-item {
	background: linear-gradient(135deg, #FFD700, #FFA500) !important;
	color: #1A1A1A !important;
	font-weight: bold !important;
	margin: 8px 12px !important;
	border-radius: 8px !important;
	box-shadow: 0 2px 8px rgba(255, 215, 0, 0.3) !important;
}

.golden-menu-item:hover {
	background: linear-gradient(135deg, #FFED4E, #FFB84D) !important;
	transform: translateY(-1px) !important;
	box-shadow: 0 4px 12px rgba(255, 215, 0, 0.4) !important;
	transition: all 0.3s ease !important;
}

.golden-menu-item .v-list-item__prepend .v-icon {
	color: #1A1A1A !important;
}
</style>