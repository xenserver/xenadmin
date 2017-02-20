#!groovy

/* Copyright (c) Citrix Systems Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

/* Note: the env variables are either Jenkins built-in variables
   or own variables configured at Manage Jenkins > Configure System */

properties([
  [
    $class  : 'BuildDiscarderProperty',
    strategy: [$class: 'LogRotator', numToKeepStr: '10', artifactNumToKeepStr: '10']
  ],
  parameters(
    [
      stringParam(
        name: 'XC_BRANDING',
        defaultValue: 'citrix',
        description: 'Branding to use. Default value is citrix.'
      ),
      stringParam(
        name: 'XC_BRANCH',
        defaultValue: 'master',
        description: 'The branch to build. Default value is master.'
      ),
      booleanParam(
        name: 'SKIP_CHECKS',
        defaultValue: false,
        description: 'Skips initial checks for a faster build. Default value is false.'
      ),
      booleanParam(
        name: 'SKIP_TESTS',
        defaultValue: false,
        description: 'Skips tests for a faster build. Default value is false.'
      ),
      booleanParam(
        name: 'DEV_BUILD',
        defaultValue: false,
        description: 'Skips bumping the global build number and uploading the artifacts. Default value is false.'
      ),
      stringParam(
        name: 'BUILD_ON_NODE',
        defaultValue: 'master',
        description: 'The Jenkins node where to build. Default value is master, but ensure you set it to a node disallowing concurrent builds.'
      )
    ])
])

for (Object key : params.keySet()) {
  def keyVal = params.get(key)
  println "Build parameter ${key} set to ${keyVal}"
}

node("${params.BUILD_ON_NODE}") {
  try {

    stage('Bump build number') {
      if (params.DEV_BUILD) {
        println "Running development build. The global build number will not be incremented. The current job's build number will be used."
      } else {
        def calcBn = httpRequest httpMode: 'GET',
          url: "${env.CGI_ENDPOINT}/next-xenadmin?job=${env.JOB_NAME}&number=${env.BUILD_NUMBER}&rev=na&repo=na"

        def nextBn = calcBn.content.trim()
        println "Got next build number: " + nextBn

        httpRequest httpMode: 'POST',
          contentType: 'APPLICATION_FORM',
          requestBody: "nextBuildNumber=${nextBn}",
          url: "${env.JOB_URL}nextbuildnumber/submit",
          authentication: 'authentication'
      }
    }

    stage('Clean workspace') {
      deleteDir()
    }

    String GIT_COMMIT_XENADMIN =''

    stage('Checkout sources') {
      checkout([
        $class           : 'GitSCM',
        branches         : [[name: "refs/heads/${params.XC_BRANCH}"]],
        extensions       : [
          [$class: 'RelativeTargetDirectory', relativeTargetDir: 'xenadmin.git'],
          [$class: 'LocalBranch', localBranch: '**'],
          [$class: 'CleanCheckout'],
          [$class: 'CloneOption', noTags: false, reference: '', shallow: true]
        ],
        userRemoteConfigs: scm.userRemoteConfigs
      ])

      GIT_COMMIT_XENADMIN = bat(
        returnStdout: true,
        script: """
                @echo off 
                cd ${env.WORKSPACE}\\xenadmin.git
                git rev-parse HEAD
                """
      ).trim()

      if (params.XC_BRANDING == 'citrix') {
        GString BRANDING_REMOTE = "${env.CODE_ENDPOINT}/xs/branding.git"

        def branchExistsOnBranding = bat(
          returnStatus: true,
          script: """git ls-remote --heads ${BRANDING_REMOTE} | grep ${params.XC_BRANCH}"""
        )
        String branchToClone = (branchExistsOnBranding == 0) ?  params.XC_BRANCH : 'master'

        bat """git clone -b ${branchToClone} ${BRANDING_REMOTE} ${env.WORKSPACE}\\branding.git"""
      } else {

        println "Overwriting Branding folder"
        GString BRAND_REMOTE = "${env.CODE_ENDPOINT}/xsc/xenadmin-branding.git"

        def branchExistsOnBrand = bat(
          returnStatus: true,
          script: """git ls-remote --heads ${BRAND_REMOTE} | grep ${params.XC_BRANCH}"""
        )
        String branchToClone = (branchExistsOnBrand == 0) ?  params.XC_BRANCH : 'master'

        bat """
            git clone -b ${branchToClone} ${BRAND_REMOTE} ${env.WORKSPACE}\\xenadmin-branding.git
            rmdir /s /q "${env.WORKSPACE}\\xenadmin.git\\Branding"
            xcopy /e /y "${env.WORKSPACE}\\xenadmin-branding.git\\${XC_BRANDING}\\*" "${env.WORKSPACE}\\xenadmin.git\\Branding\\"
        """

        println "Checking out branding specifics"
        GString BRANDING_REMOTE = "${env.CODE_ENDPOINT}/xs/${XC_BRANDING}-branding.git"

        def branchExistsOnBranding = bat(
          returnStatus: true,
          script: """git ls-remote --heads ${BRANDING_REMOTE} | grep ${params.XC_BRANCH}"""
        )
        String branchToCloneB = (branchExistsOnBranding == 0) ?  params.XC_BRANCH : 'master'

        bat """
            git clone -b ${branchToCloneB} ${BRANDING_REMOTE} ${env.WORKSPACE}\\${XC_BRANDING}-branding.git
        """
      }
    }

    def CTX_SIGN_DEFINED = bat(
      returnStdout: true,
      script: """
              @echo off
              if defined c (echo 1) else (echo 0)
              """
    ).trim()

    stage('Download dependencies') {
      GString dotNetFile = (CTX_SIGN_DEFINED == '1') ? 'DOTNET_BUILD_LOCATION_CTXSIGN' : 'DOTNET_BUILD_LOCATION'

      GString remoteDotnet = GString.EMPTY
      remoteDotnet += readFile("${env.WORKSPACE}\\xenadmin.git\\packages\\${dotNetFile}").trim()
      GString downloadSpec = GString.EMPTY
      downloadSpec += readFile("${env.WORKSPACE}\\xenadmin.git\\mk\\deps-map.json").trim().replaceAll("@REMOTE_DOTNET@", remoteDotnet)

      def server = Artifactory.server('repo')
      server.download(downloadSpec)

      println "Branding for " + params.XC_BRANDING

      if (params.XC_BRANDING == 'citrix') {
        println "Downloading hotfixes."

        def remoteUrl = server.url
        String remoteBranch = params.XC_BRANCH

        try {
          httpRequest httpMode: 'GET', url: "${remoteUrl}/builds/xs/hotfixes/${remoteBranch}"
        }
        catch (Exception ex) {
          println "Hotfixes for ${remoteBranch} not found. Trying trunk instead."
          httpRequest httpMode: 'GET', url: "${remoteUrl}/builds/xs/hotfixes/trunk"
          remoteBranch = 'trunk'
        }

        GString hotFixSpec = GString.EMPTY
        hotFixSpec += readFile("${env.WORKSPACE}\\xenadmin.git\\mk\\hotfix-map.json").trim().replaceAll("@BRANCH@", remoteBranch)
        server.download(hotFixSpec)
      }
    }

    stage('Create manifest') {
      GString manifestFile = "${env.WORKSPACE}\\output\\manifest"
      File file = new File(manifestFile)

      String branchInfo = (params.XC_BRANCH == 'master') ? 'trunk' : params.XC_BRANCH
      file << "@branch=${branchInfo}\n"
      file << "xenadmin xenadmin.git ${GIT_COMMIT_XENADMIN}\n"

      if (params.XC_BRANDING == 'citrix') {
        def SERVER_BRANDING_TIP = bat(
          returnStdout: true,
          script: """
                @echo off 
                cd ${env.WORKSPACE}\\branding.git
                git rev-parse HEAD
                """
        ).trim()

        file << "branding branding.git ${SERVER_BRANDING_TIP}\n"

      } else {
        def XENADMIN_BRANDING_TIP = bat(
          returnStdout: true,
          script: """
                @echo off 
                cd ${env.WORKSPACE}\\xenadmin-branding.git
                git rev-parse HEAD
                """
        ).trim()

        file << "xenadmin-branding xenadmin-branding.git ${XENADMIN_BRANDING_TIP}\n"

        def XC_BRANDING_TIP = bat(
          returnStdout: true,
          script: """
                @echo off 
                cd ${env.WORKSPACE}\\${XC_BRANDING}-branding.git
                git rev-parse HEAD
                """
        ).trim()

        file << "${XC_BRANDING}-branding ${XC_BRANDING}-branding.git ${XC_BRANDING_TIP}\n"
      }

      //for the time being we download a fixed version of the ovf fixup iso, hence put this in the manifest
      file << "xencenter-ovf xencenter-ovf.git 21d3d7a7041f15abfa73f916e5fd596fd7e610c4\n"
      file << "chroot-lenny chroots.hg 1a75fa5848e8\n"

      file << readFile("${env.WORKSPACE}\\scratch\\dotnet-packages-manifest").trim()
    }

    stage('Run checks') {
      if (params.SKIP_CHECKS) {
        println "Skipping initial checks on request."
      } else {

        List<String> list = ["check-roaming.sh", "copyrightcheck/copyrightcheck.sh", "i18ncheck/i18ncheck.sh", "deadcheck/deadcheck.sh", "spellcheck/spellcheck.sh"]
        for (String item : list) {
          bat """
          cd ${env.WORKSPACE}\\xenadmin.git\\devtools
          sh "${item}"
          """
        }
      }
    }

    stage('Build') {
      bat """
          cd ${env.WORKSPACE}
          sh "xenadmin.git/mk/xenadmin-build.sh"
          """
    }

    stage('Run tests') {

      if (params.XC_BRANDING != 'citrix') {
        println "Testing package-and-sign script"
        bat """
            cd ${env.WORKSPACE}
            mkdir TestXenAdminUnsigned
            unzip -q -o output\\XenAdminUnsigned.zip -d TestXenAdminUnsigned
            sh "TestXenAdminUnsigned/XenAdminUnsigned/mk/package-and-sign.sh"
            """
      }

      if (params.SKIP_TESTS) {
        println "Skipping tests on request."
      } else {
        timeout(time: 60, unit: 'MINUTES') {

          bat """
              mkdir ${env.WORKSPACE}\\tmp
              cp -r ${env.WORKSPACE}\\xenadmin.git\\XenAdminTests\\bin\\Release ${env.WORKSPACE}\\tmp
              cd ${env.WORKSPACE}\\tmp
              taskkill /f /fi "imagename eq nunit*"              
              echo Starting tests at %time% %date%
              
              nunit-console /nologo /labels /stoponerror /nodots /process=separate /noshadow /labels /err="${env.WORKSPACE}\\tmp\\error.nunit.log" /timeout=40000 /xml="${env.WORKSPACE}\\tmp\\XenAdminTests.xml" "${env.WORKSPACE}\\tmp\\Release\\XenAdminTests.dll" /framework=net-4.5
              
              echo Finished tests at %time% %date%              
              cp ${env.WORKSPACE}\\tmp\\XenAdminTests.xml ${env.WORKSPACE}\\output
              """

          def text = readFile("${env.WORKSPACE}\\output\\XenAdminTests.xml")
          assert text.contains('errors="0" failures="0"')
        }
      }
    }

    stage('Upload') {
      if (!params.DEV_BUILD) {
        dir("${env.WORKSPACE}\\output") {

          if (params.XC_BRANDING == 'citrix') {
            bat """del /f /q "${env.WORKSPACE}\\output\\XenAdminUnsigned.zip" """
          }

          def server = Artifactory.server('repo')
          def buildInfo = Artifactory.newBuildInfo()
          buildInfo.env.filter.addInclude("*")
          buildInfo.env.collect()
          //buildInfo.retention maxBuilds: 50, deleteBuildArtifacts: true

          GString artifactMeta = "build.name=${env.JOB_NAME};build.number=${env.BUILD_NUMBER};vcs.url=${env.CHANGE_URL};vcs.branch=${params.XC_BRANCH};vcs.revision=${GIT_COMMIT_XENADMIN}"

          String targetSubRepo = (CTX_SIGN_DEFINED == '1') ? 'xenadmin-ctxsign' : 'xenadmin'

          // IMPORTANT: do not forget the slash at the end of the target path!
          GString targetPath = "xc-local-build/${targetSubRepo}/${params.XC_BRANCH}/${params.XC_BRANDING}/${env.BUILD_NUMBER}/"
          GString uploadSpec = """ {
              "files": [
                { "pattern": "*", "flat": "false", "target": "${targetPath}", "props": "${artifactMeta}" }
              ]}
          """

          def buildInfo_upload = server.upload(uploadSpec)
          buildInfo.append buildInfo_upload
          server.publishBuildInfo buildInfo
        }
      }
    }

    currentBuild.result = 'SUCCESS'

  } catch (Exception ex) {
    currentBuild.result = 'FAILURE'
    throw ex as java.lang.Throwable
  } finally {
    currentBuild.displayName = "${params.XC_BRANCH}-${params.XC_BRANDING}-${env.BUILD_NUMBER}"
    step([
      $class                  : 'Mailer',
      notifyEveryUnstableBuild: true,
      recipients              : "${env.XENCENTER_DEVELOPERS}",
      sendToIndividuals       : true])
  }
}
