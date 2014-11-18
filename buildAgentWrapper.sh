#!/bin/bash

set -o nounset -o errexit

#given buildAgent.properties.template
export BUILD_AGENT_PROPERTIES="/var/teamcity/build-agent/conf/buildAgent.properties" 

#if buildAgent.properties does not exist
if [ ! -f $BUILD_AGENT_PROPERTIES ]; then 
      #copy template to propeties location
        cp /tmp/buildAgent.properties $BUILD_AGENT_PROPERTIES
    fi


    #delete config lines with sed
    sed -i '/serverUrl/d' $BUILD_AGENT_PROPERTIES

    #load configs based on current OT_ENV
    if [[ "$OT_ENV" =~ "prod" ]]; then
          cat /app/src/config/prod/app.config >> $BUILD_AGENT_PROPERTIES 
      fi
      if [[ "$OT_ENV" =~ "preprod" ]]; then
            cat /app/src/config/pp/app.config >> $BUILD_AGENT_PROPERTIES 
        fi
        if [[ "$OT_ENV" =~ "ci" ]]; then
                cat /app/src/config/ci/app.config >> $BUILD_AGENT_PROPERTIES
          fi

        #delete config line with agent name
        sed -i '/name=/d' $BUILD_AGENT_PROPERTIES
        #add outside machine's hostname
        echo `hostname` >> $BUILD_AGENT_PROPERTIES

        #run agent
        exec /var/teamcity/build-agent/bin/agent.sh run
