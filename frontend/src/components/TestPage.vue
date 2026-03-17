<template>
    <div class="content">
        <div v-if="err" class="error">
            <p><b>{{err}}</b></p>
        </div>
        <div v-if="success" class="success">
            <p><b>Success.</b></p>
        </div>
        <form @submit.prevent="submitForm">
            <table class="centered">
                <tr>
                    <td><label>LogType</label></td>
                    <td>
                        <select name="LogType" v-model="formData.logType">
                            <option value="0">None -> Creates Player</option>
                            <option value="1">Logged In</option>
                            <option value="2">Played Match</option>
                            <option value="3">Received Reward</option>
                            <option value="4">Earned Points</option>
                            <option value="5">Started Crafting</option>
                        </select>
                    </td>
                </tr>
                <tr v-if="formData.logType == 0">
                    <td><label>Player Name:</label></td>
                    <td><input type="text" name="playerName" v-model="playerName" /></td>
                </tr>
                <tr v-if="formData.logType != 0">
                    <td><label>Player UUID:</label></td>
                    <td><input type="text" name="playerId" v-model="formData.playerId" /></td>
                </tr>
                <tr v-if="formData.logType == 3 || formData.logType == 5">
                    <td><label>Item UUID:</label></td>
                    <td><input type="text" name="itemId" v-model="formData.data" /></td>
                </tr>
                <tr v-if="formData.logType == 4">
                    <td><label>Score:</label></td>
                    <td><input type="text" name="score" v-model="formData.data" /></td>
                </tr>
                <tr>
                    <td></td>
                    <td><input type="button" name="Send" value="Send" @click="onSendClicked()" /></td>
                </tr>
            </table>
        </form>
    </div>
</template>

<script lang="js" setup>
    import { ref, watch } from 'vue';
    import axios from 'axios';
    const err = ref("");
    const success = ref(false);

    const formData = ref({
        logType: 0,
        playerId: '',
        data: '',
        date: null
    });
    const playerName = ref("");

    watch(formData, clearMessages, { deep: true });
    watch(playerName, clearMessages);
    function clearMessages() {
        err.value = "";
        success.value = false;
    }

    function onSendClicked() {
        success.value = false;
        if (validate()) {
            if (formData.value.logType == 0) {
                sendAddPlayer();
            }
            else {
                sendAddLog()
            }
        }
    }
    async function sendAddLog() {
        formData.value.logType = parseInt(formData.value.logType);
        try {
            const response = await axios.post('http://localhost:5292/Activity/AddActivity', formData.value, { headers: { 'Content-Type': 'application/json' } });
            response.value
            err.value = "";
            success.value = true;
        } catch (error) {
            if (error.response) {
                const status = error.response.status || 'Unknown Status';
                const message = error.response.data && error.response.data ? error.response.data : 'No Error Message';
                err.value = `Error: ${status} - ${message}`;
            }else if (error.request) {
                err.value = "Network error. Please try again later."; //no response
            } else {
                err.value = "An unexpected error occurred."; //something else happened
            }
        }
    }
    function sendAddPlayer() {
        try {
            axios.post("http://localhost:5292/Player/Create/", playerName.value,{ headers: { 'Content-Type': 'application/json' } });
            err.value = "";
            success.value = true;
        } catch (error) {
            if (error.response) {
                err.value = `Error: ${error.response.status} - ${error.response.data.message}`; //non 2xx response
            } else if (error.request) {
                err.value = "Network error. Please try again later."; //no response
            } else {
                err.value = "An unexpected error occurred."; //something else happened
            }
        }
    }

    function validate() {
        if (formData.value.logType == 0) {//none
            if (playerName.value == "") {
                err.value = "No valid PlayerName";
                return false;
            }
        }
        else {
            if (formData.value.playerId.length != 36) {
                err.value = "Not a valid PlayerId";
                return false;
            }
            if (formData.value.logType == 3 || formData.value.logType == 5) { //reward,crafting
                if (formData.value.data.length != 36) {
                    err.value = "Not a valid ItemId";
                    return false;
                }
            }
            if (formData.value.logType == 4 && (formData.value.data == "")) {//earned points
                if (formData.value.data.length == 0) {
                    err.value = "Score is not set.";
                    return false;
                }
            }
        }
        return true;
    }
</script>

<style>
    .error, .success {
        min-height: 50px;
        border: solid red 3px;
        background-color: lightcoral;
        display: flex;
        justify-content: center;
        align-items: center;
    }
    .success {
        border: solid darkgreen 3px;
        background-color: lawngreen;
    }

        .error > p, .success > p{
            margin: 0px auto;
            color: black;
        }
</style>