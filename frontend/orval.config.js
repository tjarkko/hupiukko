module.exports = {
  api: {
    input: 'http://localhost:5109/swagger/v1/swagger.json',
    output: {
      mode: 'tags-split',
      target: './api/generated/',
      client: 'react-query',
      baseUrl: '/api/proxy'
    },
  },
}; 