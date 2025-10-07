<template>
  <v-container>
    <v-row justify="center">
      <v-col lg="6">
        <v-card variant="text">
          <v-card-title class="headline">Login</v-card-title>
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
                label="Password *"
                v-model="password"
                prepend-icon="mdi-lock"
                type="password"
                :rules="passwordRules"
                required
              ></v-text-field>
              <v-alert v-if="errorMessage" type="error">{{ errorMessage }}</v-alert>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn variant="flat" @click="signUp">Sign Up</v-btn>
            <v-btn variant="elevated" color="primary" @click="login" :disabled="!valid">Login</v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from '@/utils/axiosInstance';
import { store } from '@/utils/store';
import { parseJwt } from '@/utils/jwtParser';

export default {
  data() {
    return {
      valid: false,
      username: '',
      usernameRules: [
        v => !!v,
      ],
      password: '',
      passwordRules: [
        v => !!v,
      ],
      errorMessage: ''
    };
  },
  methods: {
    login() {
      if (!this.valid) {
        return;
      }

      axios.post('/stakeholders/users/login', {
        username: this.username,
        password: this.password
      })
      .then(response => {
        const { accessToken } = response.data;
        const tokenData = parseJwt(accessToken);
        
        console.log('Token data:', tokenData);
        console.log('Role from token:', tokenData['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']);
        
        localStorage.setItem('token', accessToken);
        store.setUser({
          username: tokenData.username,
          role: tokenData['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || tokenData.role
        });
        console.log('Store role set to:', store.role);
        console.log('Login successful:', response.data.username);
        this.$router.push("/");
      })
      .catch(error => {
        console.error('Login error:', error);
        this.errorMessage = error.response?.data?.message || 'Login failed';
      });
    },
    signUp() {
      this.$router.push("/signup");
    }
  }
};
</script>

<style scoped>

.v-container {
  margin-top: 10vh;
}

.v-card {
  padding: 3%;
}

</style>