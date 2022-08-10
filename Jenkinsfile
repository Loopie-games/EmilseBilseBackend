pipeline {
    agent any

    triggers {
        pollSCM "*/5 * * * *"
    }

    stages {
        steps ("Clean up test-results"){
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
                sh "dotnet test --collect:'XPlat Code Coverage'"
            }
            post {
                archiveArtifacts "moonbaboon.bingo.core.test/TestResults/*/coverage.cobertura.xml"
                archiveArtifacts "moonbaboon.bingo.DataAccess.Test/TestResults/*/coverage.cobertura.xml"
                archiveArtifacts "moonbaboon.bingo.Domain.Test/TestResults/*/coverage.cobertura.xml"
                archiveArtifacts "moonbaboon.bingo.WebApi.Test/TestResults/*/coverage.cobertura.xml"
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