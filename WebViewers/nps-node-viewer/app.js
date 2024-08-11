const express = require('express');
const bodyParser = require('body-parser');
const ejs = require('ejs');

const app = express();
const port = 3000;

// Middleware
app.use(bodyParser.urlencoded({ extended: false }));
app.set('view engine', 'ejs');
app.use(express.static('public'));
const axios = require('axios');

// Mock data
const users = {
    user1: {
        password: 'password1',
        projects: [
            { id: 1, name: 'Project A', summary: 'This is the summary of Project A.' },
            { id: 2, name: 'Project B', summary: 'This is the summary of Project B.' },
        ]
    }
};

// Routes
app.get('/', (req, res) => {
    res.render('login', { error: null });
});

app.post('/login', (req, res) => {
    const username = req.body.username;
    const password = req.body.password;

    if (users[username] && users[username].password === password) {
        res.redirect(`/projects/${username}`);
    } else {
        res.status(401).render('login', { error: 'Invalid username or password.' });
    }
});

app.get('/projects/:username', async (req, res) => {
    const username = req.params.username;
    try {
        const response = await axios.get(`http://localhost:5189/api/projects/${username}`);
        const projects = response.data;
        res.render('projects', { username, projects });
    } catch (error) {
        console.error(error);
        res.status(500).send('Failed to fetch projects.');
    }
});

// Start server
app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
});