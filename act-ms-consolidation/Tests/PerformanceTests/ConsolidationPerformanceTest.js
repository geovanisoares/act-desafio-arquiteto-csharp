import http from 'k6/http';
import { sleep, check } from 'k6';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

export let options = {
    stages: [
        { duration: '15s', target: 50 },  // Ramp-up to 50 users
        { duration: '30s', target: 50 },  // Sustain for 30s
        { duration: '15s', target: 0 },   // Ramp-down to 0 users
    ],
};

const BASE_URL = 'http://localhost/Consolidation';
const dates = ['2025-05-03', '2025-05-04', '2025-05-05', '2025-05-06', '2025-05-07'];

export default function () {
    let randomDate = dates[randomIntBetween(0, dates.length - 1)];
    console.log("randomDate:", randomDate);
    let url = `${BASE_URL}/${randomDate}`;
    console.log("url:", url);
    let response = http.get(url);

    check(response, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });

    sleep(1);
}
