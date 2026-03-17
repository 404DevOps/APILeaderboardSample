<template>
    <div class="post">
        <div v-if="loading" class="loading">
            Loading...
        </div>

        <div>
            <form v-if="!uuid" class="playerIdForm">
                <label>Filter by Player ID</label>
                <input class="playerIdField" type="text" name="playerId" v-model="inputValue" />
            </form>
        </div>

        <div class="content">
            <div v-if="err" class="error">
                <p><b>{{err}}</b></p>
            </div>
            <table class="centered">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Date</th>
                        <th>LogType</th>
                        <th>PlayerId</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody v-if="activities.length > 0" >
                    <tr v-for="activity in activities" :key="activity.id">
                        <td>{{ activity.id }}</td>
                        <td>{{ new Date(activity.date).toLocaleString() }}</td>
                        <td>{{ getLogTypeString(activity.logType) }}</td>
                        <td>{{ activity.playerId }}</td>
                        <td>{{ activity.data }}</td>
                    </tr>

                </tbody>
                <tbody v-else>
                     <tr>
                        <td colspan="5">No PlayerActivities found.</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script lang="js" setup>
    import { ref, watch, defineProps, onMounted } from 'vue';
    import axios from 'axios';

    const props = defineProps({
        uuid: String,
    });

    const err = ref("");
    const inputValue = ref('');
    const loading = ref(false);
    const activities = ref([]);

    // Watch both 'uuid' and 'inputValue' for changes
    watch([() => props.uuid, inputValue], ([newUuid, newInputValue]) => {
        const filterValue = newUuid || newInputValue || ''; // Use 'inputValue' if provided, otherwise 'uuid'
        loadData(filterValue);
    });

    const loadData = async (filterValue) => {
        activities.value = [];
        loading.value = true;

        try {
            const response = await axios.get('https://localhost:7196/Activity/' + filterValue);
            activities.value = response.data;
            err.value = "";
        } catch (error) {
            err.value = error;
        } finally {
            loading.value = false;
        }
    };

    // Watch for changes in 'uuid' using 'post' flush option
    watch(() => props.uuid, (newUuid) => {
        if (newUuid) {
            loadData(newUuid);
        }
    }, { flush: 'post' });


    onMounted(() => {
        if (props.uuid || inputValue.value) {
            loadData(props.uuid || inputValue.value);
        } else
        {
            loadData('');
        }
    });

    function getLogTypeString(logtype) {
        switch (logtype) {
            case 0: return "NONE";
            case 1: return "LOGGED IN";
            case 2: return "PLAYED MATCH";
            case 3: return "RECEIVED REWARD";
            case 4: return "EARNED POINTS";
            case 5: return "CRAFTED ITEM";
            default: return "UNKNOWN";
        }
    }
</script>
<style>
    .playerIdField {
        margin-left: 10px;
    }

    .playerIdForm {
        margin-bottom: 10px;
    }
</style>