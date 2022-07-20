pipeline {
    agent any

    triggers {
        pollSCM "*/5 * * * *"
    }

    stages {

        stage ("Hello World!") {
            steps {
                sh "echo 'hello world!'"
            }
        }

        stage ("Build Backend") {
            steps {
                sh "dotnet build ."
            }
        }

        stage ("Reset Containers") {
            steps {
                script {
                    try {
                        sh "docker compose --env-file config/Test.env down"
                    }
                    finally {}
                }
            }
        }

        stage ("Deploy Backend") {
            steps {
                sh "docker compose --env-file config/Test.env up -d --build"
            }
        }

        stage('Push image to register') {
            steps {
                sh "docker compose --env-file ./config/Test.env push"
            }
        }
    }
}