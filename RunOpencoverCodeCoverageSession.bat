.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -mergebyhash -skipautoprops -register:user -output:opencoverTestCoverageReport.xml -target:".\packages\NUnit.ConsoleRunner.3.5.0\tools\nunit3-console.exe" -targetargs:"VisualMutator.sln --config=Release --result:nunit3TestResult.xml --result:nunit2TestResult.xml;format=nunit2"