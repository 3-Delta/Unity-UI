# Quality Report

## QA Owner: @davidla

## Test strategy

| Test                                                                                       									 | Coverage                                                         						 | Passing                                            					
|--------------------------------------------------------------------------------------------|------------------------------------------------------------------|----------------------------------------------------|
| Native Packge builds                                                     												 | gitlab CI build process                                                       	 | passes							                 					
| Able to integrate perf test API into project																	 | unit test                                 												 | passes
| User can save performance data																					 | unit test                            	  												 | passes
| API is able to report on performance data															 		 | unit test		                                                    					 | passes											 					

(1) Katana tests are run on packages/traceeventprofiler_tests
(2) To run Katana tests on a new package, do the following:
	a. Run the pipeline on gitlab CI for your branch of the package
	b. Download the artifact from the create_package step
	c. Drop the artifact from gitlab into the Assets folder of the Playmode test project "TraceEventProfiler" (overwrite everything already in the Assets folder)
	d. Push changes
	e. Run the following tests on Katana:
	   - Test PlayModeTest - Android (ARM64 - IL2CPP)
	   - Test PlayModeTest - Android (ARM)
	   - Test PlayModeTest - iOS
	   - Test PlayModeTest - Windows64Standalone
	   - Test PlayModeTest - StandaloneOSX
	f. View hoarder links for the branch to confirm the tests ran and passed