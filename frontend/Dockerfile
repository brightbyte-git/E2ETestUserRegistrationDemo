# Use a Node.js image as the base
FROM node:18

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY package*.json ./

# Install dependencies
CMD ["npm", "install"]

# Copy the rest of the app's files
COPY . .

# Expose port 3000 for the React development server
EXPOSE 3000

# Default command
CMD ["npm", "start"]