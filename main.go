package main

import (
	"fmt"
	"log"
	"net/http"

	"github.com/joho/godotenv"
	"go-api/controller"
	"go-api/db"
	"go-api/router"
	"go-api/service"
)

func main() {
	// load .env (server env variables)
	_ = godotenv.Load()

	// 1) Connect to Postgres
	conn, err := db.NewDB()
	if err != nil {
		log.Fatalf("DB connection failed: %v", err)
	}
	defer func() {
		if cerr := conn.Close(); cerr != nil {
			log.Printf("Error closing DB connection: %v", cerr)
		}
	}()

	// 2) Wire up service & controller
	userService := service.NewUserService(conn)
	userController := controller.NewFuelController(userService)

	// 3) Register routes and start server
	router.RegisterRoutes(userController)
	fmt.Println("Server running on http://127.0.0.1:8080")
	log.Fatal(http.ListenAndServe("127.0.0.1:8080", nil))
}
