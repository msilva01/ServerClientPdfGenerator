export enum JobProcessingCallbacksNames {
  reportReady = "reportReady",
  reportError = "reportError",
}

export type JobProcessingCallBacks = {
  [JobProcessingCallbacksNames.reportReady]: (reportDto: ReportDto) => void;
  [JobProcessingCallbacksNames.reportError]: (
    userId: string,
    errorMessage: string
  ) => void;
};

export interface JobProcessingProcess {
  callbacksName: JobProcessingCallbacksNames;
  callbacks: JobProcessingCallBacks;
  methodsName: "";
  methods: {};
}

export interface ReportDto {
  id: string;
  fileName: string;
  userId: string;
}
