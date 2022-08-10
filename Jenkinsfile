pipeline {
    agent any

    triggers {
        pollSCM "*/5 * * * *"
    }

    stages {
        stage ("Clean up test-results"){
            dir("moonbaboon.bingo.core.test"){
                sh "rm -rf TestResults"
            }
            dir("moonbaboon.bingo.DataAccess.Test"){
                sh "rm -rf TestResults"
            }
            dir("moonbaboon.bingo.Domain.Test"){
                sh "rm -rf TestResults"
            }
            dir("moonbaboon.bingo.WebApi.Test"){
                sh "rm -rf TestResults"
            }
        }

        stage ("Build Backend") {
            steps {
                sh "dotnet build ."
            }
        }

        stage ("Testing backend") {
            steps {
                dir("moonbaboon.bingo.core.test"){
                    sh "dotnet test --collect:'XPlat Code Coverage'"
                }
                dir("moonbaboon.bingo.DataAccess.Test"){
                    sh "dotnet test --collect:'XPlat Code Coverage'"
                }
                dir("moonbaboon.bingo.Domain.Test"){
                    sh "dotnet test --collect:'XPlat Code Coverage'"
                }
                dir("moonbaboon.bingo.WebApi.Test"){
                    sh "dotnet test --collect:'XPlat Code Coverage'"
                }
            }
            post {
                archiveArtifacts "moonbaboon.bingo.core.test/TestResults/*/"
                archiveArtifacts "moonbaboon.bingo.DataAccess.Test/TestResults/*/"
                archiveArtifacts "moonbaboon.bingo.Domain.Test/TestResults/*/"
                archiveArtifacts "moonbaboon.bingo.WebApi.Test/TestResults/*/"
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