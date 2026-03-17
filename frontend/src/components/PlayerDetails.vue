<template>
    <div v-if="err" class="error">
        <p><b>{{err}}</b></p>
    </div>
    <h1>{{playerName}}'s Profile</h1>

    <h3>Score</h3>
    <p><b>{{score}}</b></p>
    <table class="centered">
        <tr><td><label>From</label></td><td><input type="datetime-local" name="from" v-model="fromDateInput" /></td></tr>
        <tr><td><label>To</label></td><td><input type="datetime-local" name="to" v-model="toDateInput" /></td></tr>
    </table>
    <h3>Player Activity</h3>
    <activities :uuid="uuid"></activities>
</template>

<script lang="js" setup>
    import { ref, onMounted, watch} from 'vue'
    import { useRoute } from 'vue-router'
    import axios from 'axios';
    import activities from './ActivitiesList.vue';

    // Declare a ref for 'uuid' to be used in the template
    const uuid = ref('');
    const playerName = ref('');
    const err = ref('');
    const score = ref(0);
    const fromDate = ref(null);
    const fromDateInput = ref('');
    const toDate = ref(null);
    const toDateInput = ref('');
    // Define the onMounted hook
    onMounted(() => {
        const route = useRoute();
        uuid.value = route.params.playerId; // Update the 'uuid' ref with the value
        console.log(uuid.value);
        getName();
        getScore();
    });

    watch([fromDateInput, toDateInput], () => { parseDate(); getScore() });  

    async function getName() {
        playerName.value = '';

        const response = await axios.get('http://localhost:5292/Player/' + uuid.value).catch(function (error) { err.value = error; return; });
        playerName.value = response.data.name;
        err.value = "";
    }
    async function getScore() {
        score.value = 0;
        const response = await axios.get('http://localhost:5292/Activity/GetScore/', {
            params: {
                playerId: uuid.value,
                fromDate: fromDate.value ? fromDate.value : null,
                toDate: toDate.value ? toDate.value : null
            }
        }).catch(function (error) { err.value = error; return; });
        score.value = response.data;
        err.value = "";
    }

    function parseDate() {
        const fromInputValue = fromDateInput.value;
        if (fromInputValue) {
            fromDate.value = new Date(fromInputValue + 'Z');
        } else {
            fromDate.value = null;
        }
        const toInputValue = toDateInput.value;
        if (toInputValue) {
            toDate.value = new Date(toInputValue + 'Z');
        } else {
            toDate.value = null;
        }
    }

</script>