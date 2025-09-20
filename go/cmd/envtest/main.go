package main

import (
	"fmt"
	"os"
)

func main() {
	fmt.Println("EIA_API_KEY:", os.Getenv("EIA_API_KEY"))
}
