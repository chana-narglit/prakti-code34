import axios from 'axios';

const apiUrl =  "https://prakti-code34server.onrender.com/task";
axios.defaults.baseURL = apiUrl;

axios.interceptors.response.use(
  response => response,
  error => {
    console.error("שגיאה ב-response:", error.response ? error.response.data : "Unknown error");
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get('/');
    return result.data;
  },

  addTask: async (taskName) => {
    const response = await axios.post('/', {
      name: taskName,
      isComplete: false,
    });
    return response.data;
  },


  setCompleted: async (  todo, isComplete) => {
    try {
   
      const result = await axios.put(`/${todo.id}?isDone=${isComplete}`, {
            id: todo.id,
            name: todo.name
        });
      return { result };
    } catch (error) {
      console.error("שגיאה בעידכון משימה:", error);
      return { error: error.response ? error.response.data : "Unknown error" };
    }
  },

  deleteTask: async (id) => {
    await axios.delete(`/${id}`);
  }
};
