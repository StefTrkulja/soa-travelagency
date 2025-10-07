<template>
  <v-container>
    <v-row justify="center">
      <v-col lg="6">
        <v-card variant="text">
          <v-card-title class="headline">Travel Agency Sign Up</v-card-title>
          <v-spacer></v-spacer>
          <v-card-text>
            <v-form v-model="valid">
              <v-text-field
                label="Username *"
                v-model="username"
                prepend-icon="mdi-account"
                :rules="usernameRules"
                required
              ></v-text-field>
              <br>
              <v-text-field
                label="Email *"
                v-model="email"
                prepend-icon="mdi-email"
                type="email"
                :rules="emailRules"
                required
              ></v-text-field>
              <br>
              <v-text-field
                label="Password *"
                v-model="password"
                prepend-icon="mdi-lock"
                type="password"
                :rules="passwordRules"
                required
              ></v-text-field>
              <br>
              <v-text-field
                label="Confirm password *"
                v-model="confirmPassword"
                prepend-icon="mdi-lock-outline"
                type="password"
                :rules="confirmPasswordRules"
                required
              ></v-text-field>
              <br>
              <v-select
                label="Role *"
                v-model="userRole"
                prepend-icon="mdi-account-group"
                :items="roleOptions"
                item-title="text"
                item-value="value"
                :rules="roleRules"
                required
              ></v-select>
              <br>
              <v-alert v-if="errorMessage" type="error">{{ errorMessage }}</v-alert>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn variant="elevated" color="primary" size="large" @click="signUp" :disabled="!valid">Sign Up</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
		<v-snackbar v-model="snackbar" :timeout="snackbarTimeout" color="success">
      {{ snackbarMessage }}
    </v-snackbar>
  </v-container>
</template>

<script>
import axios from '@/utils/axiosInstance';
import { store } from '@/utils/store';
import { parseJwt } from '@/utils/jwtParser';

export default {
  computed: {
    store() {
      return store;
    }
  },
  data() {
    return {
      valid: false,
			snackbar: false,
			snackbarMessage: '',
			snackbarTimeout: 2500,
      username: '',
      usernameRules: [
        v => !!v || 'Username is required',
        v => v.length >= 3 || 'Username must be at least 3 characters',
      ],
      email: '',
      emailRules: [
        v => !!v || 'Email is required',
        v => /.+@.+\..+/.test(v) || 'Email must be valid',
      ],
      password: '',
      passwordRules: [
        v => !!v || 'Password is required',
        v => v.length >= 6 || 'Password must be at least 6 characters',
      ],
      confirmPassword: '',
      confirmPasswordRules: [
        v => !!v || 'Confirm password is required',
        v => v === this.password || 'Passwords must match',
      ],
      userRole: 2, // Default to Tourist
      roleOptions: [
        { text: 'Tourist', value: 2 },
        { text: 'Author', value: 1 }
      ],
      roleRules: [
        v => v !== null && v !== undefined || 'Role is required',
      ],
      errorMessage: ''
    };
  },
  methods: {
    signUp() {
      axios.post('/stakeholders/users/register', {
        username: this.username,
        email: this.email,
        password: this.password,
        userRole: this.userRole
      })
      .then(response => {
        const { accessToken } = response.data;
        const tokenData = parseJwt(accessToken);
        
        localStorage.setItem('token', accessToken);
        store.setUser({
          username: tokenData.username,
          role: tokenData['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || tokenData.role
        });
        
				this.snackbarMessage = 'Account successfully created!';
				this.snackbar = true;
				this.resetForm();
        setTimeout(() => {
					this.$router.push("/");
				}, this.snackbarTimeout);
      })
      .catch(error => {
        this.errorMessage = error.response?.data?.message || 'Registration failed';
      });
    },
		resetForm() {
			this.username = '';
			this.email = '';
			this.password = '';
			this.confirmPassword = '';
      this.userRole = 2;
			this.errorMessage = '';
			this.valid = false;
		}
  }
};
</script>

<style scoped>
.v-container {
  margin-top: 2vh;
}

.v-card {
  padding: 3%;
}
</style>