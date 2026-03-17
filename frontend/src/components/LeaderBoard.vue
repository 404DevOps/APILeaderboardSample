<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading...
        </div>

        <div class="content">
            <div v-if="err" class="error">
                <p><b>{{err}}</b></p>
            </div>
            <table class="centered">
                <thead>
                    <tr>
                        <th>Rank</th>
                        <th>Name</th>
                        <th>UUID</th>
                        <th>Score</th>
                    </tr>
                </thead>
                <tbody v-if="playerList.length > 0" >
                    <tr v-for="(player, index) in playerList" :key="player.name">
                        <td>#{{ index + 1}}</td>
                        <td><router-link :to="`/details/${player.uuid}`">{{ player.name }}</router-link></td>
                        <td>{{ player.uuid }}</td>
                        <td>{{ player.score }}</td>
                    </tr>
              
                </tbody>
                <tbody v-else >
                    <tr>
                        <td colspan="4">No Leaderboard Entries found.</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script lang="js" setup>
    import { ref } from 'vue';
    import axios from 'axios';

    const err = ref("");
    const loading = ref(false)
    const playerList = ref([])

    const loadData = async () => {
        playerList.value = [];
        loading.value = true;

        const response = await axios.get('http://localhost:5292/Player/GetLeaderboard').catch(function (error) { err.value = error; return; });
        playerList.value = response.data;
        loading.value = false;
        err.value = "";
    }

    loadData();
</script>